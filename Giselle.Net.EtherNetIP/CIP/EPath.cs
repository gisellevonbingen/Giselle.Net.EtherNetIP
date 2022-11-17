using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class EPath : List<IEPathSegment>, IEquatable<EPath>
    {
        public static EPath FromANSI(string value) => new EPath() { EPathSegmentSymbolicANSI.FromValue(value) };

        public static IEPathSegment CreateSegment(byte type)
        {
            if (type == EPathSegmentSymbolicANSI.Base)
            {
                return new EPathSegmentSymbolicANSI();
            }
            else if (EPathSegmentLogical.RangeStart <= type && type <= EPathSegmentLogical.RangeEnd)
            {
                return new EPathSegmentLogical() { TypeBase = EPathSegmentLogical.ToTypeBase(type) };
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Type({type}) is out of range for read a PathSegment");
            }

        }

        public bool HasReserved { get; set; }

        public byte Reserved { get; set; }

        public EPath()
        {

        }

        public EPath(IEnumerable<IEPathSegment> collection) : base(collection)
        {

        }

        public EPath(DataProcessor processor) : this()
        {
            this.Read(processor);
        }

        public void Read(DataProcessor processor)
        {
            this.Clear();

            var wordCount = processor.ReadByte();

            if (this.HasReserved == true)
            {
                this.Reserved = processor.ReadByte();
            }

            var wordBytes = processor.ReadBytes(wordCount * 2);

            using (var segmentStream = new MemoryStream(wordBytes))
            {
                var segmentProcessor = CIPCodec.CreateDataProcessor(segmentStream);

                while (segmentStream.Position < segmentStream.Length)
                {
                    var segmentType = processor.ReadByte();
                    var segment = CreateSegment(segmentType);
                    segment.ReadValue(segmentType, segmentProcessor);
                    this.Add(segment);
                }

                if (segmentStream.Position != segmentStream.Length)
                {
                    throw new EPathException($"{nameof(EPath)} should read more segments");
                }

            }

        }

        public void Write(DataProcessor processor)
        {
            using (var segmentStream = new MemoryStream())
            {
                var segmentProcessor = CIPCodec.CreateDataProcessor(segmentStream);

                foreach (var segment in this)
                {
                    segmentProcessor.WriteByte(segment.Type);
                    segment.WriteValue(segmentProcessor);
                }

                var wordBytes = segmentStream.ToArray();

                if (wordBytes.Length % 2 > 0)
                {
                    throw new EPathException($"{nameof(EPath)}'s bytes was not padded in words");
                }

                var wordCount = wordBytes.Length / 2;
                processor.WriteByte((byte)wordCount);

                if (this.HasReserved == true)
                {
                    processor.WriteByte(this.Reserved);
                }

                processor.WriteBytes(wordBytes);
            }

        }

        public override int GetHashCode()
        {
            var hash = 17;

            foreach (var segment in this)
            {
                hash = hash * 31 + segment.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is EPath other && this.Equals(other);
        }

        public bool Equals(EPath other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            return this.SequenceEqual(other);
        }

    }

}
