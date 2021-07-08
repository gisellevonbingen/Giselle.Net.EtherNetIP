using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum Priority : byte
    {
        Low = 0,
        High = 1,
        Scheduled = 2,
        Urgent = 3,
    }

}
