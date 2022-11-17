using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public enum ConnectionType : byte
    {
		Null = 0,
		Multicast = 1,
		PointToPoint = 2,
    }

}
