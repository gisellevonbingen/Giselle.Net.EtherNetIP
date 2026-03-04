using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class ForwardOpenResult
    {
        public ushort Error { get; set; }
        public ExtendedStatusError ExtendedStatus { get; set; }
        public ushort ConnectionSerialNumber { get; set; }

        public IPv4EndPoint O_T_Address { get; set; }
        public IPv4EndPoint T_O_Address { get; set; }
        public uint O_T_ConnectionId { get; set; }
        public uint T_O_ConnectionId { get; set; }

        public ForwardOpenResult()
        {

        }

        public ForwardOpenResult(ForwardOpenResult other)
        {
            this.Error = other.Error;
            this.ExtendedStatus = other.ExtendedStatus;
            this.ConnectionSerialNumber = other.ConnectionSerialNumber;

            this.O_T_Address = other.O_T_Address == null ? null : new IPv4EndPoint(other.O_T_Address);
            this.T_O_Address = other.T_O_Address == null ? null : new IPv4EndPoint(other.T_O_Address);
            this.O_T_ConnectionId = other.O_T_ConnectionId;
            this.T_O_ConnectionId = other.T_O_ConnectionId;
        }

    }

}
