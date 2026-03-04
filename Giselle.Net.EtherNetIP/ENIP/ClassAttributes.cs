using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class ClassAttributes
    {
        public Func<AttributePath, DataProcessor> GetAttribute { get; private set; }
        public uint ClassId { get; }

        public ClassAttributes(Func<AttributePath, DataProcessor> getAttribute, uint classId)
        {
            this.GetAttribute = getAttribute;
            this.ClassId = classId;
        }

        public DataProcessor Read(uint attributeId = 0) => this.GetAttribute(new AttributePath(this.ClassId, 0, attributeId));

        public ushort Revision => this.Read(KnownClassAttributeId.Revision).ReadUShort();

        public ushort InstanceMaxId => this.Read(KnownClassAttributeId.InstanceMaxId).ReadUShort();

        public ushort InstanceCount => this.Read(KnownClassAttributeId.InstanceCount).ReadUShort();

        public ushort ClassAttributesMaxId => this.Read(KnownClassAttributeId.ClassAttributesMaxId).ReadUShort();

        public ushort InstanceAttributesMaxId => this.Read(KnownClassAttributeId.InstanceAttributesMaxId).ReadUShort();

    }

}
