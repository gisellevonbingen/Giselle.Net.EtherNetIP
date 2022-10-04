using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class CommandItemConnectedData : CommandItem
    {
        public MemoryStream DataStream { get; private set; }
        public DataProcessor DataProcessor { get; private set; }

        public CommandItemConnectedData()
        {
            this.DataStream = new MemoryStream();
            this.DataProcessor = ENIPCodec.CreateDataProcessor(this.DataStream);
        }

        public override void Read(DataProcessor processor)
        {
            base.Read(processor);

            this.DataStream.Capacity = 0;
            processor.BaseStream.CopyTo(this.DataStream);
            this.DataStream.Position = 0;
        }

        public override void Write(DataProcessor processor)
        {
            base.Write(processor);

            processor.WriteBytes(this.DataStream.ToArray());
        }

    }

}
