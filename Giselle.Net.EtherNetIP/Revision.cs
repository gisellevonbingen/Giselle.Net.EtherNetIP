using System;
using System.Collections.Generic;
using System.Text;

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

        public Revision(DataProcessor processor) : this()
        {
            this.Read(processor);
        }

        public void Read(DataProcessor processor)
        {
            this.Major = processor.ReadByte();
            this.Minor = processor.ReadByte();
        }

        public void Write(DataProcessor processor)
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
            var hash = 17;
            hash = hash * 31 + this.Major.GetHashCode();
            hash = hash * 31 + this.Minor.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is Revision other && this.Equals(other);
        }

        public bool Equals(Revision other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
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
