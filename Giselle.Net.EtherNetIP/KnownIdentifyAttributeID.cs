using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public static class KnownIdentifyAttributeID
    {
        public const ushort VenderID = 1;
        public const ushort DeviceType = 2;
        public const ushort ProductCode = 3;
        public const ushort Revision = 4;
        public const ushort Status = 5;
        public const ushort SerialNumber = 6;
        public const ushort ProductName = 7;
        /// <summary>
        /// Optional
        /// </summary>
        public const ushort State = 8;
        /// <summary>
        /// Optional
        /// </summary>
        public const ushort ConfigurationConsistencyValue = 9;
        /// <summary>
        /// Optional
        /// </summary>
        public const ushort HeartbeatInterval = 10;
    }

}
