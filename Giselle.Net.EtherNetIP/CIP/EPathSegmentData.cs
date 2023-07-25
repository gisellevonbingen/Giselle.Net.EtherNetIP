using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP.CIP
{
    public struct EPathSegmentData : IEquatable<EPathSegmentData>, IEPathSegment
    {
        public const byte SegmentType = 0x04;

        public const byte DataTypeMask = 0x3F;
        public const byte DataTypeOffset = 0;

        public static Encoding SymbolicEncoding => Encoding.ASCII;

        public static byte ToDataType(byte typeAssembly) => (byte)((typeAssembly & DataTypeMask) >> DataTypeOffset);

        public static EPathSegmentData FromSymbolicANSI(string value) => new EPathSegmentData(KnownEPathDataSegmentType.SymbolicANSI, SymbolicEncoding.GetBytes(value));

        public byte DataType { get; set; }
        public byte[] Payload { get; set; }

        public EPathSegmentData(byte dataType, byte[] payload) : this()
        {
            this.DataType = dataType;
            this.Payload = payload;
        }

        public byte TypeAssembly
        {
            get
            {
                return EPath.MergeSegmentTypeAssembly(SegmentType, (byte)(DataTypeMask & (this.DataType << DataTypeOffset)));
            }

        }

        public void ReadValue(byte readingType, DataProcessor processor)
        {
            var length = processor.ReadByte();
            this.Payload = processor.ReadBytes(length);

            // Pad to words
            processor.ReadBytes(length % 2);
        }

        public void WriteValue(DataProcessor processor)
        {
            var payload = this.Payload;
            processor.WriteByte((byte)payload.Length);
            processor.WriteBytes(payload);

            // Pad to words
            processor.WriteBytes(new byte[payload.Length % 2]);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.DataType.GetHashCode();
            hash = hash * 31 + this.Payload.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is EPathSegmentData other && this.Equals(other);
        }

        public bool Equals(EPathSegmentData other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            if (this.DataType != other.DataType)
            {
                return false;
            }

            if (this.Payload.SequenceEqual(other.Payload) == false)
            {
                return false;
            }

            return true;
        }

    }

}
