using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class AbstractOptions
    {
        /// <summary>
        /// Actual Timeout = (2 ^ TickTime) * TimeoutTicks
        /// </summary>
        public byte TickTime { get; set; }
        public byte TimeoutTicks { get; set; }

        public ushort ConnectionSerialNumber { get; set; }
        public ushort OriginatorVenderID { get; set; }
        public uint OriginatorSerialNumber { get; set; }
        /// <summary>
        /// 2 ^ (2 + multifiler)
        /// </summary>
        public byte TimeoutMultiplier { get; set; }

        public uint ClassID { get; set; }

        public AbstractOptions()
        {
            this.TickTime = 3;
            this.TimeoutTicks = 250;

            this.ConnectionSerialNumber = 0;
            this.OriginatorVenderID = 0x00FF;
            this.OriginatorSerialNumber = 0xFFFFFFFF;
            this.TimeoutMultiplier = 3;

            this.ClassID = KnownClassID.Assembly;
        }

        public AbstractOptions(AbstractOptions other)
        {
            this.TickTime = other.TickTime;
            this.TimeoutTicks = other.TimeoutTicks;

            this.ConnectionSerialNumber = other.ConnectionSerialNumber;
            this.OriginatorVenderID = other.OriginatorVenderID;
            this.OriginatorSerialNumber = other.OriginatorSerialNumber;
            this.TimeoutMultiplier = other.TimeoutMultiplier;

            this.ClassID = other.ClassID;
        }

    }

}
