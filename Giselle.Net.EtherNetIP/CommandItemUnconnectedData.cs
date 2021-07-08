using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public abstract class CommandItemUnconnectedData : CommandItem
    {
        public byte Command { get; set; }
        public MemoryStream DataStream { get; private set; }
        public ENIPProcessor DataProcessor { get; private set; }

        public CommandItemUnconnectedData()
        {
            this.DataStream = new MemoryStream();
            this.DataProcessor = new ENIPProcessor(this.DataStream);
        }

        public override void Read(ENIPProcessor processor)
        {
            base.Read(processor);

            this.ReadHeader(processor);
            this.DataStream.Capacity = 0;
            processor.BaseStream.CopyTo(this.DataStream);
            this.DataStream.Position = 0;
        }

        public override void Write(ENIPProcessor processor)
        {
            base.Write(processor);

            this.WriteHeader(processor);
            processor.WriteBytes(this.DataStream.ToArray());
        }

        protected virtual void ReadHeader(ENIPProcessor processor)
        {
            this.Command = processor.ReadByte();
        }

        protected virtual void WriteHeader(ENIPProcessor processor)
        {
            processor.WriteByte(this.Command);
        }

    }

    public class CommandItemUnconnectedDataRequest : CommandItemUnconnectedData
    {
        public RequestPath Path { get; set; }

        public CommandItemUnconnectedDataRequest()
        {

        }

        protected override void ReadHeader(ENIPProcessor processor)
        {
            base.ReadHeader(processor);

            this.Path = processor.ReadRequestPath();
        }

        protected override void WriteHeader(ENIPProcessor processor)
        {
            base.WriteHeader(processor);

            processor.WriteRequestPath(this.Path);
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

        protected override void ReadHeader(ENIPProcessor processor)
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

        protected override void WriteHeader(ENIPProcessor processor)
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
