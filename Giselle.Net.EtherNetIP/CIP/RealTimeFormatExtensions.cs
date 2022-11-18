using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public static class RealTimeFormatExtensions
    {
        public static ushort GetFlagsBonus(this RealTimeFormat realTimeFormat)
        {
            if (realTimeFormat == RealTimeFormat.Header32Bit)
            {
                return 6;
            }
            else if (realTimeFormat == RealTimeFormat.Heartbeat)
            {
                return 0;
            }
            else
            {
                return 2;
            }

        }

    }

}
