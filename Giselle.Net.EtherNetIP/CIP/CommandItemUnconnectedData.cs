using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public abstract class CommandItemUnconnectedData : CommandItem
    {
        public ServiceCode ServiceCode { get; set; }
        public MemoryStream DataStream { get; private set; }
        public DataProcessor DataProcessor { get; private set; }

        public CommandItemUnconnectedData()
        {
            this.DataStream = new MemoryStream();
            this.DataProcessor = CIPCodec.CreateDataProcessor(this.DataStream);
        }

        public override void Read(DataProcessor processor)
        {
            base.Read(processor);

            this.ReadHeader(processor);
            this.DataStream.Capacity = 0;
            processor.BaseStream.CopyTo(this.DataStream);
            this.DataStream.Position = 0;
        }

        public override void Write(DataProcessor processor)
        {
            base.Write(processor);

            this.WriteHeader(processor);
            processor.WriteBytes(this.DataStream.ToArray());
        }

        protected virtual void ReadHeader(DataProcessor processor)
        {
            this.ServiceCode = (ServiceCode)processor.ReadByte();
        }

        protected virtual void WriteHeader(DataProcessor processor)
        {
            processor.WriteByte((byte)this.ServiceCode);
        }

    }

    public class CommandItemUnconnectedDataRequest : CommandItemUnconnectedData
    {
        public EPath Path { get; set; } = new EPath();

        public CommandItemUnconnectedDataRequest()
        {

        }

        protected override void ReadHeader(DataProcessor processor)
        {
            base.ReadHeader(processor);

            this.Path = processor.ReadEPath();
        }

        protected override void WriteHeader(DataProcessor processor)
        {
            base.WriteHeader(processor);

            processor.WriteEPath(this.Path);
        }

    }

    public class CommandItemUnconnectedDataResponse : CommandItemUnconnectedData
    {
        public byte Unknown1 { get; set; }
        public byte Error { get; set; }
        public ushort[] ExtendedStatus { get; set; }

        public CommandItemUnconnectedDataResponse()
        {
            this.ExtendedStatus = new ushort[0];
        }

        protected override void ReadHeader(DataProcessor processor)
        {
            base.ReadHeader(processor);

            this.Unknown1 = processor.ReadByte();
            this.Error = processor.ReadByte();

            this.ExtendedStatus = new ushort[processor.ReadByte()];

            for (var i = 0; i < this.ExtendedStatus.Length; i++)
            {
                this.ExtendedStatus[i] = processor.ReadUShort();
            }

        }

        protected override void WriteHeader(DataProcessor processor)
        {
            base.WriteHeader(processor);

            processor.WriteByte(this.Unknown1);
            processor.WriteByte(this.Error);

            processor.WriteByte((byte)this.ExtendedStatus.Length);

            for (var i = 0; i < this.ExtendedStatus.Length; i++)
            {
                processor.WriteUShort(this.ExtendedStatus[i]);
            }

        }

    }

}
