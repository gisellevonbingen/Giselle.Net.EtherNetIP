using System;
using System.Collections.Generic;
using System.Text;
using Giselle.Commons;

namespace Giselle.Net.EtherNetIP
{
    public struct Revision : IEquatable<Revision>
    {
        public static bool operator ==(Revision a, Revision b)
        {
            return a.Equals(b) == true;
        }

        public static bool operator !=(Revision a, Revision b)
        {
            return a.Equals(b) == false;
        }

        public byte Major { get; set; }
        public byte Minor { get; set; }

        public void Read(ENIPProcessor processor)
        {
            this.Major = processor.ReadByte();
            this.Minor = processor.ReadByte();
        }

        public void Write(ENIPProcessor processor)
        {
            processor.WriteByte(this.Major);
            processor.WriteByte(this.Minor);
        }

        public override string ToString()
        {
            return $"{nameof(this.Major)}={this.Major}, {nameof(this.Minor)}={this.Minor}";
        }

        public override int GetHashCode()
        {
            var hash = ObjectUtils.HashSeed;
            hash = hash.AccumulateHashCode(this.Major);
            hash = hash.AccumulateHashCode(this.Minor);
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is Revision other && this.Equals(other);
        }

        public bool Equals(Revision other)
        {
            if (this.EqualsType(other) == false)
            {
                return false;
            }

            if (this.Major != other.Major)
            {
                return false;
            }

            if (this.Minor != other.Minor)
            {
                return false;
            }

            return true;
        }

    }

}
