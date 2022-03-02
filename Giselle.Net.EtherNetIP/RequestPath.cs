using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public struct RequestPath : IEquatable<RequestPath>
    {
        public const byte ClassBase = 0x20;
        public const byte InstanceBase = 0x24;
        public const byte ConnectionPointBase = 0x2C;
        public const byte AttributeBase = 0x30;

        public ushort ClassID { get; set; }
        public ushort InstanceID { get; set; }
        public ushort AttributeID { get; set; }

        public RequestPath(ushort classID, ushort instanceID, ushort attributeID = 0)
            : this()
        {
            this.ClassID = classID;
            this.InstanceID = instanceID;
            this.AttributeID = attributeID;
        }

        public void Read(ENIPProcessor processor)
        {
            this.ClassID = 0;
            this.InstanceID = 0;
            this.AttributeID = 0;

            var words = processor.ReadByte();

            for (var i = 0; i < words; i++)
            {
                var tuple = new PathSegment();
                tuple.Read(processor);

                switch (tuple.Base)
                {
                    case ClassBase: this.ClassID = tuple.ID; break;
                    case InstanceBase: this.InstanceID = tuple.ID; break;
                    case AttributeBase: this.AttributeID = tuple.ID; break;
                }

            }

        }

        public void Write(ENIPProcessor processor)
        {
            var tuples = new List<PathSegment>
            {
                new PathSegment() { Base = ClassBase, ID = this.ClassID },
                new PathSegment() { Base = InstanceBase, ID = this.InstanceID }
            };

            var attributeID = this.AttributeID;

            if (attributeID > 0)
            {
                tuples.Add(new PathSegment() { Base = AttributeBase, ID = attributeID });
            }

            processor.WriteByte((byte)tuples.Count);

            foreach (var tuple in tuples)
            {
                tuple.Write(processor);
            }

        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.ClassID.GetHashCode();
            hash = hash * 31 + this.InstanceID.GetHashCode();
            hash = hash * 31 + this.AttributeID.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is RequestPath other && this.Equals(other);
        }

        public bool Equals(RequestPath other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            if (this.ClassID != other.ClassID)
            {
                return false;
            }

            if (this.InstanceID != other.InstanceID)
            {
                return false;
            }

            if (this.AttributeID != other.AttributeID)
            {
                return false;
            }

            return true;
        }

    }

}
