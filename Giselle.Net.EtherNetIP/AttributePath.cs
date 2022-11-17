using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public struct AttributePath : IEquatable<AttributePath>
    {
        public uint ClassID { get; set; }
        public uint InstanceID { get; set; }
        public uint AttributeID { get; set; }

        public AttributePath(uint classId, uint instanceId, uint attributeId = 0) : this()
        {
            this.ClassID = classId;
            this.InstanceID = instanceId;
            this.AttributeID = attributeId;
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