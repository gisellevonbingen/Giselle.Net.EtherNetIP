using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum EncapsulationCommand : ushort
    {
        NOP = 0,
        ListServices = 4,
        ListIdentity = 99,
        ListInterfaces = 100,
        RegisterSession = 101,
        UnRegisterSession = 102,
        SendRRData = 111,
        SendUnitData = 112,
        IndicateStatus = 114,
        Cancel = 115
    }

}
