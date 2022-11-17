using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum ServiceCode : byte
    {
        NOP = 0x00,
        GetAttributeAll = 0x01,
        GetAttributeSingle = 0x0E,
        SetAttributeSingle = 0x10,
        ForwardClose = 0x4E,
        ForwardOpen = 0x54,
    }

}
