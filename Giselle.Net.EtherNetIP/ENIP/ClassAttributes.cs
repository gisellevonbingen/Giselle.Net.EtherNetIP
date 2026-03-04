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

        public DataProcessor Read(uint attributeID = 0) => this.GetAttribute(new AttributePath(this.ClassId, 0, attributeID));

        public ushort Revision => this.Read(KnownClassAttributeID.Revision).ReadUShort();

        public ushort InstanceMaxID => this.Read(KnownClassAttributeID.InstanceMaxID).ReadUShort();

        public ushort InstanceCount => this.Read(KnownClassAttributeID.InstanceCount).ReadUShort();

        public ushort ClassAttributesMaxID => this.Read(KnownClassAttributeID.ClassAttributesMaxID).ReadUShort();

        public ushort InstanceAttributesMaxID => this.Read(KnownClassAttributeID.InstanceAttributesMaxID).ReadUShort();

    }

}
