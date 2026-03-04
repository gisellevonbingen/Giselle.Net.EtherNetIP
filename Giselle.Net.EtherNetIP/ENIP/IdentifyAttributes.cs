using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class IdentifyAttributes
    {
        public const uint ClassId = KnownClassId.Identify;

        public Func<AttributePath, DataProcessor> GetAttribute { get; private set; }

        public IdentifyAttributes(Func<AttributePath, DataProcessor> getAttribute)
        {
            this.GetAttribute = getAttribute;
        }

        public DataProcessor Read(uint attributeId = 0) => this.GetAttribute(new AttributePath(ClassId, 1, attributeId));

        public ushort VenderId => this.Read(KnownIdentifyAttributeId.VenderId).ReadUShort();

        public ushort DeviceType => this.Read(KnownIdentifyAttributeId.DeviceType).ReadUShort();

        public ushort ProductCode => this.Read(KnownIdentifyAttributeId.ProductCode).ReadUShort();

        public Revision Revision => new Revision(this.Read(KnownIdentifyAttributeId.Revision));

        public ushort Status => this.Read(KnownIdentifyAttributeId.Status).ReadUShort();

        public uint SerialNumber => this.Read(KnownIdentifyAttributeId.SerialNumber).ReadUInt();

        public string ProductName
        {
            get
            {
                var processor = this.Read(KnownIdentifyAttributeId.ProductName);
                var length = processor.ReadByte();
                return Encoding.UTF8.GetString(processor.ReadBytes(length));
            }

        }

        public ClassAttributes ClassAttributes => new ClassAttributes(this.GetAttribute, ClassId);
    }

}
