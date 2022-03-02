using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    public class DataProcessor
    {
        public bool IsLittleEndian { get; set; }
        public bool IsBigEndian { get => !this.IsLittleEndian; set => this.IsLittleEndian = !value; }

        public Stream BaseStream { get; private set; }
        public long ReadLength { get; private set; }
        public long WriteLength { get; private set; }

        public DataProcessor(Stream stream)
        {
            this.IsLittleEndian = BitConverter.IsLittleEndian;
            this.BaseStream = stream;
        }

        public virtual bool CanRead { get { return this.BaseStream.CanRead; } }

        public virtual bool CanWrite { get { return this.BaseStream.CanWrite; } }

        public virtual bool CanSeek { get { return this.BaseStream.CanSeek; } }

        public virtual bool CanTimeout { get { return this.BaseStream.CanTimeout; } }

        public virtual long Position
        {
            get { return this.BaseStream.Position; }
            set { this.BaseStream.Position = value; }
        }

        public virtual long Length { get { return this.BaseStream.Length; } }

        public virtual long Remain { get { return this.Length - this.Position; } }

        public virtual void Write(byte[] bytes, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                this.WriteByte(bytes[offset + i]);
            }

        }

        public virtual void WriteByte(byte value)
        {
            this.BaseStream.WriteByte(value);
            this.WriteLength++;
        }

        public virtual void WriteBytes(byte[] value)
        {
            this.Write(value, 0, value.Length);
        }
        public virtual void WriteSByte(sbyte value)
        {
            this.WriteByte((byte)value);
        }

        public virtual void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteUShort(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteChar(char value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteUInt(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteULong(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual void WriteGuid(Guid value)
        {
            var bytes = value.ToByteArray();
            this.FlipCheck(bytes);
            this.WriteBytes(bytes);
        }

        public virtual int Read(byte[] bytes, int offset, int count)
        {
            int length = 0;

            for (var i = 0; i < count; i++)
            {
                if (this.ReadByte(out var data) == true)
                {
                    bytes[offset + i] = data;
                    length++;
                }
                else
                {
                    break;
                }

            }

            return length;
        }

        public virtual bool ReadByte(out byte data)
        {
            var d = this.BaseStream.ReadByte();

            if (d == -1)
            {
                data = 0;
                return false;
            }
            else
            {
                this.ReadLength++;
                data = (byte)d;
                return true;
            }

        }

        public virtual byte ReadByte()
        {
            if (this.ReadByte(out var data) == true)
            {
                return data;
            }
            else
            {
                throw new IOException();
            }

        }

        public virtual byte[] ReadBytes(int length)
        {
            var bytes = new byte[length];
            this.ReadBytes(bytes);

            return bytes;
        }

        public virtual void ReadBytes(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = this.ReadByte();
            }

        }

        public virtual sbyte ReadSByte()
        {
            return (sbyte)this.ReadByte();
        }

        public virtual short ReadShort()
        {
            var bytes = this.ReadBytes(2);
            this.FlipCheck(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public virtual ushort ReadUShort()
        {
            var bytes = this.ReadBytes(2);
            this.FlipCheck(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public virtual int ReadInt()
        {
            var bytes = this.ReadBytes(4);
            this.FlipCheck(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public virtual uint ReadUInt()
        {
            var bytes = this.ReadBytes(4);
            this.FlipCheck(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public virtual long ReadLong()
        {
            var bytes = this.ReadBytes(8);
            this.FlipCheck(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public virtual ulong ReadULong()
        {
            var bytes = this.ReadBytes(8);
            this.FlipCheck(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public virtual float ReadFloat()
        {
            var bytes = this.ReadBytes(4);
            this.FlipCheck(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public virtual double ReadDouble()
        {
            var bytes = this.ReadBytes(8);
            this.FlipCheck(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        public virtual Guid ReadGuid()
        {
            var bytes = this.ReadBytes(16);
            this.FlipCheck(bytes);
            return new Guid(bytes);
        }

        public virtual void FlipCheck(byte[] bytes)
        {
            if (this.IsLittleEndian != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

        }

    }

}
