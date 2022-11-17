using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public static class KnownIdentifyAttributeID
    {
        public const uint VenderID = 1;
        public const uint DeviceType = 2;
        public const uint ProductCode = 3;
        public const uint Revision = 4;
        public const uint Status = 5;
        public const uint SerialNumber = 6;
        public const uint ProductName = 7;
        /// <summary>
        /// Optional
        /// </summary>
        public const uint State = 8;
        /// <summary>
        /// Optional
        /// </summary>
        public const uint ConfigurationConsistencyValue = 9;
        /// <summary>
        /// Optional
        /// </summary>
        public const uint HeartbeatInterval = 10;
    }

}
