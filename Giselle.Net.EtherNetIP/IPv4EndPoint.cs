using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Giselle.Commons.IO;
using Giselle.Commons.Net;

namespace Giselle.Net.EtherNetIP
{
    public class IPv4EndPoint
    {
        public ushort Family { get; set; }
        public ushort Port { get; set; }
        public IPAddress Address { get; set; }
        public byte[] Reserved { get; private set; }

        public IPv4EndPoint()
        {
            this.Address = IPAddress.Any;
            this.Reserved = new byte[8];
        }

        public void Read(Stream stream)
        {
            var processor = new DataProcessor(stream) { IsBigEndian = true };
            this.Family = processor.ReadUShort();
            this.Port = processor.ReadUShort();
            this.Address = processor.ReadInt().ToIPv4Address(processor.IsBigEndian);
            processor.ReadBytes(this.Reserved);
        }

        public void Write(Stream stream)
        {
            var processor = new DataProcessor(stream) { IsBigEndian = true };
            processor.WriteUShort(this.Family);
            processor.WriteUShort(this.Port);
            processor.WriteInt(this.Address.ToIPv4Address(processor.IsBigEndian));
            processor.WriteBytes(this.Reserved);
        }

    }

}
