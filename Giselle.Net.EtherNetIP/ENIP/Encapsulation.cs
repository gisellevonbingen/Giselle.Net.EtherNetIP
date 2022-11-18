using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class Encapsulation
    {
        public EncapsulationCommand Command { get; set; }
        public MemoryStream DataStream { get; private set; }
        public DataProcessor DataProcessor { get; private set; }
        public uint SessionID { get; set; }
        public EncapsulationStatus Status { get; set; }
        public byte[] SenderContext { get; private set; }
        public uint Option { get; set; }

        public Encapsulation()
        {
            this.Command = EncapsulationCommand.NOP;
            this.DataStream = new MemoryStream();
            this.DataProcessor = ENIPCodec.CreateDataProcessor(this.DataStream);
            this.SessionID = 0;
            this.Status = EncapsulationStatus.Success;
            this.SenderContext = new byte[8];
            this.Option = 0;
        }

        public Encapsulation(DataProcessor processor) : this()
        {
            this.Read(processor);
        }

        public void Read(DataProcessor processor)
        {
            this.Command = (EncapsulationCommand)processor.ReadUShort();
            var bytes = new byte[processor.ReadUShort()];
            this.SessionID = processor.ReadUInt();
            this.Status = (EncapsulationStatus)processor.ReadUInt();
            processor.ReadBytes(this.SenderContext);
            this.Option = processor.ReadUInt();
            processor.ReadBytes(bytes);

            this.DataStream.Capacity = 0;
            this.DataStream.Write(bytes, 0, bytes.Length);
            this.DataStream.Position = 0;
        }

        public void Write(DataProcessor processor)
        {
            var data = this.DataStream.ToArray();
            processor.WriteUShort((ushort)this.Command);
            processor.WriteUShort((ushort)data.Length);
            processor.WriteUInt(this.SessionID);
            processor.WriteUInt((uint)this.Status);
            processor.WriteBytes(this.SenderContext);
            processor.WriteUInt(this.Option);

            processor.WriteBytes(data);
        }

    }

}
