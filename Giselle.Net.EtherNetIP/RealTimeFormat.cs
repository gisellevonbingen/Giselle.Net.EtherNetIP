using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum RealTimeFormat : byte
    {
        Modeless = 0,
        ZeroLength = 1,
        Heartbeat = 2,
        Header32Bit = 3,
    }

}
