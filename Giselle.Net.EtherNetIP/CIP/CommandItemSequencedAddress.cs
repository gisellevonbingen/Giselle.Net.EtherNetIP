using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class CommandItemSequencedAddress : CommandItem
    {
        public uint ConnectionID { get; set; }
        public uint SequenceCount { get; set; }

        public CommandItemSequencedAddress()
        {

        }

        public override void Read(DataProcessor processor)
        {
            base.Read(processor);

            this.ConnectionID = processor.ReadUInt();
            this.SequenceCount = processor.ReadUInt();
        }

        public override void Write(DataProcessor processor)
        {
            base.Write(processor);

            processor.WriteUInt(this.ConnectionID);
            processor.WriteUInt(this.SequenceCount);
        }

    }

}
