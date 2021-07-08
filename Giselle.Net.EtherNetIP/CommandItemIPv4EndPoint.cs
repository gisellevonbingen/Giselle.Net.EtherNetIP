using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public abstract class CommandItemIPv4EndPoint : CommandItem
    {
        public IPv4EndPoint EndPoint { get; set; }

        public CommandItemIPv4EndPoint()
        {
            this.EndPoint = new IPv4EndPoint();
        }

        public override void Read(ENIPProcessor processor)
        {
            base.Read(processor);

            this.EndPoint.Read(processor.BaseStream);
        }

        public override void Write(ENIPProcessor processor)
        {
            base.Write(processor);

            this.EndPoint.Write(processor.BaseStream);
        }

    }

    public class CommandItemEndPoint_O_T : CommandItemIPv4EndPoint
    {

    }

    public class CommandItemEndPoint_T_O : CommandItemIPv4EndPoint
    {

    }

}
