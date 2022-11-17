using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    public struct EPathSegmentSymbolicANSI : IEquatable<EPathSegmentSymbolicANSI>, IEPathSegment
    {
        public const byte Base = 0x91;
        public static Encoding Encoding => Encoding.ASCII;

        public static EPathSegmentSymbolicANSI FromValue(string value) => new EPathSegmentSymbolicANSI(value);

        public string Value { get; set; }

        public byte Type => Base;

        public EPathSegmentSymbolicANSI(string value) : this()
        {
            this.Value = value;
        }

        public void ReadValue(byte readingType, DataProcessor processor)
        {
            var length = processor.ReadByte();
            var bytes = processor.ReadBytes(length);
            this.Value = Encoding.GetString(bytes);

            // Pad to words
            processor.ReadBytes(length % 2);
        }

        public void WriteValue(DataProcessor processor)
        {
            var bytes = Encoding.GetBytes(this.Value);
            processor.WriteByte((byte)bytes.Length);
            processor.WriteBytes(bytes);

            // Pad to words
            processor.WriteBytes(new byte[bytes.Length % 2]);
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is EPathSegmentSymbolicANSI other && this.Equals(other);
        }

        public bool Equals(EPathSegmentSymbolicANSI other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            return string.Equals(this.Value, other.Value);
        }

    }

}
