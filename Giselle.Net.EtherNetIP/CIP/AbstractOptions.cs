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
        public ushort OriginatorVenderId { get; set; }
        public uint OriginatorSerialNumber { get; set; }
        /// <summary>
        /// 2 ^ (2 + multifiler)
        /// </summary>
        public byte TimeoutMultiplier { get; set; }

        public uint ClassId { get; set; }

        public AbstractOptions()
        {
            this.TickTime = 3;
            this.TimeoutTicks = 250;

            this.ConnectionSerialNumber = 0;
            this.OriginatorVenderId = 0x00FF;
            this.OriginatorSerialNumber = 0xFFFFFFFF;
            this.TimeoutMultiplier = 3;

            this.ClassId = KnownClassId.Assembly;
        }

        public AbstractOptions(AbstractOptions other)
        {
            this.TickTime = other.TickTime;
            this.TimeoutTicks = other.TimeoutTicks;

            this.ConnectionSerialNumber = other.ConnectionSerialNumber;
            this.OriginatorVenderId = other.OriginatorVenderId;
            this.OriginatorSerialNumber = other.OriginatorSerialNumber;
            this.TimeoutMultiplier = other.TimeoutMultiplier;

            this.ClassId = other.ClassId;
        }

    }

}
