using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class ForwardOpenResult
    {
        public ForwardOpenOptions Options { get; set; }

        public ushort Error { get; set; }
        public ExtendedStatusError ExtendedStatus { get; set; }
        public ushort ConnectionSerialNumber { get; set; }

        public IPv4EndPoint O_T_Address { get; set; }
        public IPv4EndPoint T_O_Address { get; set; }
        public uint O_T_ConnectionID { get; set; }
        public uint T_O_ConnectionID { get; set; }

        public ForwardOpenResult()
        {

        }

    }

}
