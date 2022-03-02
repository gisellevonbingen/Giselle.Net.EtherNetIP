using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class ENIPProcessor : DataProcessor
    {
        public ENIPProcessor(Stream stream) : base(stream)
        {
            this.IsLittleEndian = true;
        }

        public Encapsulation ReadEncapsulation()
        {
            var encapsulation = new Encapsulation();
            encapsulation.Read(this);
            return encapsulation;
        }

        public void WriteEncapsulation(Encapsulation encapsulation)
        {
            encapsulation.Write(this);
        }

        public CommandItems ReadCPacket(bool isRequest = false)
        {
            var packet = new CommandItems();
            packet.Read(this, isRequest);
            return packet;
        }

        public void WriteCommonPacket(CommandItems packet)
        {
            packet.Write(this);
        }

        public RequestPath ReadRequestPath()
        {
            var epath = new RequestPath();
            epath.Read(this);
            return epath;
        }

        public void WriteRequestPath(RequestPath epath)
        {
            epath.Write(this);
        }

        public CommandData ReadCommandData(bool isRequest = false)
        {
            var epath = new CommandData();
            epath.Read(this, isRequest);
            return epath;
        }

        public void WriteCommandData(CommandData epath)
        {
            epath.Write(this);
        }

    }

}
