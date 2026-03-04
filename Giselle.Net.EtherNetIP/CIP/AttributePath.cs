using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public struct AttributePath : IEquatable<AttributePath>
    {
        public uint ClassId { get; set; }
        public uint InstanceId { get; set; }
        public uint AttributeId { get; set; }

        public AttributePath(uint classId, uint instanceId, uint attributeId = 0) : this()
        {
            this.ClassId = classId;
            this.InstanceId = instanceId;
            this.AttributeId = attributeId;
        }

        public EPath AsEPath() => new EPath(this.AsEPathSegments());

        public IEnumerable<IEPathSegment> AsEPathSegments()
        {
            yield return EPathSegmentLogical.FromClassId(this.ClassId);
            yield return EPathSegmentLogical.FromInstanceId(this.InstanceId);

            if (this.AttributeId > 0)
            {
                yield return EPathSegmentLogical.FromAttributeId(this.AttributeId);
            }

        }

        public override string ToString() => $"{this.ClassId}.{this.InstanceId}.{this.AttributeId}";

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.ClassId.GetHashCode();
            hash = hash * 31 + this.InstanceId.GetHashCode();
            hash = hash * 31 + this.AttributeId.GetHashCode();
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

            if (this.ClassId != other.ClassId)
            {
                return false;
            }

            if (this.InstanceId != other.InstanceId)
            {
                return false;
            }

            if (this.AttributeId != other.AttributeId)
            {
                return false;
            }

            return true;
        }

    }

}