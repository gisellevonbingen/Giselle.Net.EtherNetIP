using System;
using System.Collections.Generic;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class SendRRData
    {
        public InterfaceHandle InterfaceHandle { get; set; }
        public ushort Timeout { get; set; }

        public CommandItems Items { get; private set; }

        public SendRRData()
        {
            this.InterfaceHandle = InterfaceHandle.CIP;
            this.Timeout = 0;

            this.Items = new CommandItems();
        }

        public SendRRData(params CommandItem[] collection) : this()
        {
            this.Items.AddRange(collection);
        }

        public SendRRData(IEnumerable<CommandItem> collection) : this()
        {
            this.Items.AddRange(collection);
        }

        public SendRRData(DataProcessor processor, bool isRequest) : this()
        {
            this.Read(processor, isRequest);
        }

        public void Read(DataProcessor processor, bool isRequest)
        {
            this.InterfaceHandle = (InterfaceHandle)processor.ReadUInt();
            this.Timeout = processor.ReadUShort();

            this.Items.Read(processor, isRequest);
        }

        public void Write(DataProcessor processor)
        {
            processor.WriteUInt((uint)this.InterfaceHandle);
            processor.WriteUShort(this.Timeout);

            this.Items.Write(processor);
        }

    }

}
