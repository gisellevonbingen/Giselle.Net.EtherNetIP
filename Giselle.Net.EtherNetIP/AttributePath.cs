using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public struct AttributePath : IEquatable<AttributePath>
    {
        public ushort ClassID { get; set; }
        public ushort InstanceID { get; set; }
        public ushort AttributeID { get; set; }

        public AttributePath(ushort classID, ushort instanceID, ushort attributeID = 0) : this()
        {
            this.ClassID = classID;
            this.InstanceID = instanceID;
            this.AttributeID = attributeID;
        }

        public IEnumerable<IPathSegment> AsRequestPathSegments()
        {
            yield return PathSegmentLogical.FromClassID(this.ClassID);
            yield return PathSegmentLogical.FromInstanceID(this.InstanceID);

            if (this.AttributeID > 0)
            {
                yield return PathSegmentLogical.FromAttributeID(this.AttributeID);
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
            return obj is AttributePath other && this.Equals(other);
        }

        public bool Equals(AttributePath other)
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