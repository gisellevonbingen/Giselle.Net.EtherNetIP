using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class IdentifyAttributes
    {
        public const uint ClassID = KnownClassID.Identify;

        public Func<AttributePath, DataProcessor> GetAttribute { get; private set; }

        public IdentifyAttributes(Func<AttributePath, DataProcessor> getAttribute)
        {
            this.GetAttribute = getAttribute;
        }

        public DataProcessor Read(uint attributeID = 0) => this.GetAttribute(new AttributePath(ClassID, 1, attributeID));

        public ushort VenderID => this.Read(KnownIdentifyAttributeID.VenderID).ReadUShort();

        public ushort DeviceType => this.Read(KnownIdentifyAttributeID.DeviceType).ReadUShort();

        public ushort ProductCode => this.Read(KnownIdentifyAttributeID.ProductCode).ReadUShort();

        public Revision Revision => new Revision(this.Read(KnownIdentifyAttributeID.Revision));

        public ushort Status => this.Read(KnownIdentifyAttributeID.Status).ReadUShort();

        public uint SerialNumber => this.Read(KnownIdentifyAttributeID.SerialNumber).ReadUInt();

        public string ProductName
        {
            get
            {
                var processor = this.Read(KnownIdentifyAttributeID.ProductName);
                var length = processor.ReadByte();
                return Encoding.UTF8.GetString(processor.ReadBytes(length));
            }

        }

        public ClassAttributes ClassAttributes => new ClassAttributes(this.GetAttribute, ClassID);
    }

}
