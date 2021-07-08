using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum IdentifyState : byte
    {
        NonExistent = 0,
        DeviceSelfTesting = 1,
        Standby = 2,
        Operational = 3,
        MajorRecoverableFault = 4,
        MajorUnrecoverableFault = 5,
    }

}
