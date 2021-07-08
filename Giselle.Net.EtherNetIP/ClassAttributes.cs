using System;
using System.Collections.Generic;
using System.Text;
using Giselle.Commons;

namespace Giselle.Net.EtherNetIP
{
    public struct ClassAttributes : IEquatable<ClassAttributes>
    {
        public static bool operator ==(ClassAttributes a, ClassAttributes b)
        {
            return a.Equals(b) == true;
        }

        public static bool operator !=(ClassAttributes a, ClassAttributes b)
        {
            return a.Equals(b) == false;
        }

        public ushort Revision { get; set; }

        public ushort MaximumInstance { get; set; }

        public ushort ClassAttributesMaxID { get; set; }

        public ushort InstanceAttributesMaxID { get; set; }

        public void Read(ENIPProcessor processor)
        {
            this.Revision = processor.ReadUShort();
            this.MaximumInstance = processor.ReadUShort();
            this.ClassAttributesMaxID = processor.ReadUShort();
            this.InstanceAttributesMaxID = processor.ReadUShort();
        }

        public void Write(ENIPProcessor processor)
        {
            processor.WriteUShort(this.Revision);
            processor.WriteUShort(this.MaximumInstance);
            processor.WriteUShort(this.ClassAttributesMaxID);
            processor.WriteUShort(this.InstanceAttributesMaxID);
        }

        public override int GetHashCode()
        {
            var hash = ObjectUtils.HashSeed;
            hash = hash.AccumulateHashCode(this.Revision);
            hash = hash.AccumulateHashCode(this.MaximumInstance);
            hash = hash.AccumulateHashCode(this.ClassAttributesMaxID);
            hash = hash.AccumulateHashCode(this.InstanceAttributesMaxID);
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is ClassAttributes other && this.Equals(other);
        }

        public bool Equals(ClassAttributes other)
        {
            if (this.EqualsType(other) == false)
            {
                return false;
            }

            if (this.Revision != other.Revision)
            {
                return false;
            }

            if (this.MaximumInstance != other.MaximumInstance)
            {
                return false;
            }

            if (this.ClassAttributesMaxID != other.ClassAttributesMaxID)
            {
                return false;
            }

            if (this.InstanceAttributesMaxID != other.InstanceAttributesMaxID)
            {
                return false;
            }

            return true;
        }

    }

}
