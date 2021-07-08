using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public static class KnownClassID
    {
        public const ushort Identify = 0x0001;
        public const ushort MessageRouter = 0x0002;
        public const ushort Assembly = 0x0004;
        public const ushort Connection = 0x0005;
        public const ushort ConnectionManager = 0x0006;
        public const ushort Register = 0x0007;
        public const ushort Parameter = 0x000F;
        public const ushort ParameterGroup = 0x0010;
        public const ushort AcknowledgeHandler = 0x002B;
        public const ushort Selection = 0x2E;
        public const ushort File = 0x0037;
        public const ushort OriginatorConnectionList = 0x0045;
        public const ushort ConnectionConfiguration = 0x00F3;
        public const ushort Port = 0x00F4;
    }

}
