using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public struct IdentifyAttributes : IEquatable<IdentifyAttributes>
    {
        public static bool operator ==(IdentifyAttributes a, IdentifyAttributes b)
        {
            return a.Equals(b) == true;
        }

        public static bool operator !=(IdentifyAttributes a, IdentifyAttributes b)
        {
            return a.Equals(b) == false;
        }

        public ushort VenderID { get; set; }
        public ushort DeviceType { get; set; }
        public ushort ProductCode { get; set; }
        public Revision Revision { get; set; }
        public ushort Status { get; set; }
        public uint SerialNumber { get; set; }
        public string ProductName { get; set; }

        public IdentifyAttributes(ENIPProcessor processor) : this()
        {
            this.Read(processor);
        }

        public void Read(ENIPProcessor processor)
        {
            this.VenderID = processor.ReadUShort();
            this.DeviceType = processor.ReadUShort();
            this.ProductCode = processor.ReadUShort();
            this.Revision = new Revision(processor);
            this.Status = processor.ReadUShort();
            this.SerialNumber = processor.ReadUInt();

            var produectNameLength = processor.ReadByte();
            this.ProductName = Encoding.UTF8.GetString(processor.ReadBytes(produectNameLength));
        }

        public void Write(ENIPProcessor processor)
        {
            processor.WriteUShort(this.VenderID);
            processor.WriteUShort(this.DeviceType);
            processor.WriteUShort(this.ProductCode);
            processor.WriteUShort(this.Status);
            processor.WriteUInt(this.SerialNumber);

            processor.WriteByte((byte)this.ProductName.Length);
            processor.WriteBytes(Encoding.UTF8.GetBytes(this.ProductName));
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.VenderID.GetHashCode();
            hash = hash * 31 + this.DeviceType.GetHashCode();
            hash = hash * 31 + this.ProductCode.GetHashCode();
            hash = hash * 31 + this.Revision.GetHashCode();
            hash = hash * 31 + this.Status.GetHashCode();

            hash = hash * 31 + this.SerialNumber.GetHashCode();
            hash = hash * 31 + this.ProductName?.GetHashCode() ?? 0;
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is IdentifyAttributes other && this.Equals(other);
        }

        public bool Equals(IdentifyAttributes other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            if (this.VenderID != other.VenderID)
            {
                return false;
            }

            if (this.DeviceType != other.DeviceType)
            {
                return false;
            }

            if (this.ProductCode != other.ProductCode)
            {
                return false;
            }

            if (this.Revision != other.Revision)
            {
                return false;
            }

            if (this.Status != other.Status)
            {
                return false;
            }

            if (this.SerialNumber != other.SerialNumber)
            {
                return false;
            }

            if (this.ProductName != other.ProductName)
            {
                return false;
            }

            return true;
        }

    }

}
