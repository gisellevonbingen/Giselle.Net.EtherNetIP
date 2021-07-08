using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class AssemblyObject
    {
        public uint ConnectionID { get; set; }
        public byte InstanceID { get; set; }
        public byte Length { get; set; }
        public bool OwnerRedundant { get; set; }
        public bool VariableLength { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public Priority Priority { get; set; }
        public RealTimeFormat RealTimeFormat { get; set; }
        public uint RequestPacketRate { get; set; }

        public AssemblyObject()
        {
            this.ConnectionID = 0;
            this.InstanceID = 0;
            this.Length = 0;
            this.OwnerRedundant = false;
            this.VariableLength = false;
            this.ConnectionType = ConnectionType.Null;
            this.Priority = Priority.Low;
            this.RealTimeFormat = RealTimeFormat.Modeless;
            this.RequestPacketRate = 500000;
        }

        public AssemblyObject(AssemblyObject other)
        {
            this.ConnectionID = other.ConnectionID;
            this.InstanceID = other.InstanceID;
            this.Length = other.Length;
            this.OwnerRedundant = other.OwnerRedundant;
            this.VariableLength = other.VariableLength;
            this.ConnectionType = other.ConnectionType;
            this.Priority = other.Priority;
            this.RealTimeFormat = other.RealTimeFormat;
            this.RequestPacketRate = other.RequestPacketRate;
        }

        public ushort Flags
        {
            get
            {
                var num4 = this.Length + this.RealTimeFormat.GetFlagsBonus();
                var num5 = num4 & 0x01FF;
                num5 |= Convert.ToUInt16(this.VariableLength) << 0x09;
                num5 |= Convert.ToUInt16(this.Priority) << 0x0A;
                num5 |= Convert.ToUInt16(this.ConnectionType) << 0x0D;
                num5 |= Convert.ToUInt16(this.OwnerRedundant) << 0x0F;

                return (ushort)num5;
            }

        }

    }

}
