using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class IdentifyObject
    {
        public ENIPCodec Parent { get; private set; }
        public Stream BaseStream { get; private set; }

        public IdentifyObject(ENIPCodec parent, Stream stream)
        {
            this.Parent = parent;
            this.BaseStream = stream;
        }

        public ENIPProcessor Read(ushort attributeID = 0)
        {
            return this.Parent.GetAttribute(this.BaseStream, new RequestPath(KnownClassID.Identify, 1, attributeID));
        }

        public ushort VenderID { get { return this.Read(KnownIdentifyAttributeID.VenderID).ReadUShort(); } }

        public ushort DeviceType { get { return this.Read(KnownIdentifyAttributeID.DeviceType).ReadUShort(); } }

        public ushort ProductCode { get { return this.Read(KnownIdentifyAttributeID.ProductCode).ReadUShort(); } }

        public Revision Revision { get { return new Revision(this.Read(KnownIdentifyAttributeID.Revision)); } }

        public ushort Status { get { return this.Read(KnownIdentifyAttributeID.Status).ReadUShort(); } }

        public uint SerialNumber { get { return this.Read(KnownIdentifyAttributeID.SerialNumber).ReadUInt(); } }

        public string ProductName
        {
            get
            {
                var processor = this.Read(KnownIdentifyAttributeID.ProductName);
                var length = processor.ReadByte();
                return Encoding.UTF8.GetString(processor.ReadBytes(length));
            }

        }

        /// <summary>
        /// Optional
        /// </summary>
        public IdentifyState State { get { return (IdentifyState)this.Read(KnownIdentifyAttributeID.SerialNumber).ReadByte(); } }

        /// <summary>
        /// Optional
        /// </summary>
        public ushort ConfigurationConsistencyValue { get { return this.Read(KnownIdentifyAttributeID.ConfigurationConsistencyValue).ReadUShort(); } }

        /// <summary>
        /// Optional
        /// </summary>
        public byte HeartbeatInterval { get { return this.Read(KnownIdentifyAttributeID.HeartbeatInterval).ReadByte(); } }

        public ClassAttributes ClassAttributes { get { return new ClassAttributes(this.Parent.GetAttribute(this.BaseStream, new RequestPath(KnownClassID.Identify, 0))); } }

        public IdentifyAttributes IdentifyAttributes { get { return new IdentifyAttributes(this.Read()); } }

    }

}
