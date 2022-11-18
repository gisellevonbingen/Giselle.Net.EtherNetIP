using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class ForwardCloseResult
    {
        public ForwardCloseOptions Options { get; set; }

        public ushort Error { get; set; }
        public ExtendedStatusError ExtendedStatus { get; set; }

        public ForwardCloseResult()
        {

        }

    }

}
