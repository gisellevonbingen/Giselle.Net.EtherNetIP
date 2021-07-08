using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum CommonPacketCommand : byte
    {
        NOP = 0,
        GetAttributeAll = 1,
        GetAttributeSingle = 14,
        SetAttribute = 16,
        ForwardClose = 78,
        ForwardOpen = 84,
    }

}
