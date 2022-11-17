using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class ClassAttributes
    {
        public ENIPCodec Parent { get; private set; }
        public Stream BaseStream { get; private set; }
        public uint ClassId { get; }

        public ClassAttributes(ENIPCodec parent, Stream stream, uint classId)
        {
            this.Parent = parent;
            this.BaseStream = stream;
            this.ClassId = classId;
        }

        public DataProcessor Read(uint attributeID = 0)
        {
            return this.Parent.GetAttribute(this.BaseStream, new AttributePath(this.ClassId, 0, attributeID));
        }

        public ushort Revision => this.Read(KnownClassAttributeID.Revision).ReadUShort();

        public ushort InstanceMaxID => this.Read(KnownClassAttributeID.InstanceMaxID).ReadUShort();

        public ushort InstanceCount => this.Read(KnownClassAttributeID.InstanceCount).ReadUShort();

        public ushort ClassAttributesMaxID => this.Read(KnownClassAttributeID.ClassAttributesMaxID).ReadUShort();

        public ushort InstanceAttributesMaxID => this.Read(KnownClassAttributeID.InstanceAttributesMaxID).ReadUShort();

    }

}
