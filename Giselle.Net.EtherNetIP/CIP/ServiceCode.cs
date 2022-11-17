using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public enum ServiceCode : byte
    {
        NOP = 0x00,
        GetAttributeAll = 0x01,
        SetAttributeAll = 0x02,
        GetAttributeSingle = 0x0E,
        SetAttributeSingle = 0x10,
        ForwardClose = 0x4E,
        ForwardOpen = 0x54,
    }

}
