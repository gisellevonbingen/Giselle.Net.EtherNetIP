using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class PathSegments : List<IPathSegment>, IEquatable<PathSegments>
    {
        public static PathSegments FromANSI(string value) => new PathSegments() { PathSegmentSymbolicANSI.FromValue(value) };

        public static IPathSegment CreateSegment(byte type)
        {
            if (type == PathSegmentSymbolicANSI.Base)
            {
                return new PathSegmentSymbolicANSI();
            }
            else if (PathSegmentLogical.RangeStart <= type && type <= PathSegmentLogical.RangeEnd)
            {
                return new PathSegmentLogical() { TypeBase = PathSegmentLogical.ToTypeBase(type) };
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Type({type}) is out of range for read a PathSegment");
            }

        }

        public bool HasReserved { get; set; }

        public byte Reserved { get; set; }

        public PathSegments()
        {

        }

        public PathSegments(IEnumerable<IPathSegment> collection) : base(collection)
        {

        }

        public PathSegments(DataProcessor processor) : this()
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
                var segmentProcessor = ENIPCodec.CreateDataProcessor(segmentStream);

                while (segmentStream.Position < segmentStream.Length)
                {
                    var segmentType = processor.ReadByte();
                    var segment = CreateSegment(segmentType);
                    segment.ReadValue(segmentType, segmentProcessor);
                    this.Add(segment);
                }

                if (segmentStream.Position != segmentStream.Length)
                {
                    throw new PathSegmentException("PathSegments should read more segments");
                }

            }

        }

        public void Write(DataProcessor processor)
        {
            using (var segmentStream = new MemoryStream())
            {
                var segmentProcessor = ENIPCodec.CreateDataProcessor(segmentStream);

                foreach (var segment in this)
                {
                    segmentProcessor.WriteByte(segment.Type);
                    segment.WriteValue(segmentProcessor);
                }

                var wordBytes = segmentStream.ToArray();

                if (wordBytes.Length % 2 > 0)
                {
                    throw new PathSegmentException("PathSegments's bytes was not padded in words");
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
            return obj is PathSegments other && this.Equals(other);
        }

        public bool Equals(PathSegments other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            return this.SequenceEqual(other);
        }

    }

}
