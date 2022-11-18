using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public enum EncapsulationStatus : uint
    {
        Success = 0,
        InvalidCommand = 1,
        InsufficientMemory = 2,
        IncorrectData = 3,
        InvalidSessionHandle = 100,
        InvalidLength = 101,
        UnsupportedEncapsulationProtocol = 105,
    }

}
