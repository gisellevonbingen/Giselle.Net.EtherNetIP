﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class IdentifyAttributes
    {
        public const uint ClassID = KnownClassID.Identify;

        public ENIPCodec Parent { get; private set; }
        public Stream BaseStream { get; private set; }

        public IdentifyAttributes(ENIPCodec parent, Stream stream)
        {
            this.Parent = parent;
            this.BaseStream = stream;
        }

        public DataProcessor Read(uint attributeID = 0)
        {
            return this.Parent.GetAttribute(this.BaseStream, new AttributePath(ClassID, 1, attributeID));
        }

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

        public ClassAttributes ClassAttributes => this.Parent.GetClassAttributes(this.BaseStream, ClassID);
    }

}
