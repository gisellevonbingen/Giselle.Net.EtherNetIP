using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public static class KnownClassID
    {
        public const uint Identify = 0x0001;
        public const uint MessageRouter = 0x0002;
        public const uint Assembly = 0x0004;
        public const uint Connection = 0x0005;
        public const uint ConnectionManager = 0x0006;
        public const uint Register = 0x0007;
        public const uint Parameter = 0x000F;
        public const uint ParameterGroup = 0x0010;
        public const uint AcknowledgeHandler = 0x002B;
        public const uint Selection = 0x2E;
        public const uint File = 0x0037;
        public const uint OriginatorConnectionList = 0x0045;
        public const uint ConnectionConfiguration = 0x00F3;
        public const uint Port = 0x00F4;
    }

}
