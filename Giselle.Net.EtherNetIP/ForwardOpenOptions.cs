using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class ForwardOpenOptions : AbstractOptions
    {
        /// <summary>
        /// Originator -> Target, Output
        /// </summary>
        public AssemblyObject O_T_Assembly { get; private set; }
        /// <summary>
        /// Target -> Originator, Input
        /// </summary>
        public AssemblyObject T_O_Assembly { get; private set; }

        public ushort T_O_UDPPort { get; set; }
        public IPAddress LocalAddress { get; set; }

        public ForwardOpenOptions()
        {
            this.O_T_Assembly = new AssemblyObject() { InstanceID = KnownAssemblyInstanceID.PrimaryOutput };
            this.T_O_Assembly = new AssemblyObject() { InstanceID = KnownAssemblyInstanceID.PrimaryInput };

            this.T_O_UDPPort = 2222;
            this.LocalAddress = IPAddress.Loopback;
        }

        public ForwardOpenOptions(ForwardOpenOptions other)
            : base(other)
        {
            this.O_T_Assembly = new AssemblyObject(other.O_T_Assembly);
            this.T_O_Assembly = new AssemblyObject(other.T_O_Assembly);

            this.T_O_UDPPort = other.T_O_UDPPort;
            this.LocalAddress = other.LocalAddress;
        }

    }

}
