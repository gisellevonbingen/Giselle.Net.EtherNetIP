using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public struct EPathSegmentLogical : IEquatable<EPathSegmentLogical>, IEPathSegment
    {
        public const byte RangeStart = 0x20;
        public const byte RangeEnd = 0x3F;

        public const byte ClassIDBase = 0x20;
        public const byte InstanceIDBase = 0x24;
        public const byte ElementIDBase = 0x28;
        public const byte ConnectionPointIDBase = 0x2C;
        public const byte AttributeIDBase = 0x30;

        public static byte ToTypeBase(byte readingType) => (byte)((readingType + 3) % 4);

        public static EPathSegmentLogical FromClassID(uint value) => new EPathSegmentLogical(ClassIDBase, value);

        public static EPathSegmentLogical FromInstanceID(uint value) => new EPathSegmentLogical(InstanceIDBase, value);

        public static EPathSegmentLogical FromElementID(uint value) => new EPathSegmentLogical(ElementIDBase, value);

        public static EPathSegmentLogical FromConnectionPointID(uint value) => new EPathSegmentLogical(ConnectionPointIDBase, value);

        public static EPathSegmentLogical FromAttributeID(uint value) => new EPathSegmentLogical(AttributeIDBase, value);

        public byte TypeBase { get; set; }
        public uint Value { get; set; }

        public EPathSegmentLogical(byte typeBase, uint value = 0)
            : this()
        {
            this.TypeBase = typeBase;
            this.Value = value;
        }

        public byte Type
        {
            get
            {
                var typeBase = this.TypeBase;
                var id = this.Value;

                if (id > ushort.MaxValue)
                {
                    return (byte)(typeBase + 2);
                }
                else if (id > byte.MaxValue)
                {
                    return (byte)(typeBase + 1);
                }
                else
                {
                    return (byte)(typeBase + 0);
                }

            }

        }

        public void ReadValue(byte readingType, DataProcessor processor)
        {
            var mod = readingType % 4;

            if (mod == 0)
            {
                this.Value = processor.ReadByte();
            }
            else if (mod == 1)
            {
                // Pad to words
                processor.ReadByte();
                this.Value = processor.ReadUShort();
            }
            else if (mod == 2)
            {
                // Pad to words
                processor.ReadByte();
                this.Value = processor.ReadUInt();
            }
            else if (mod == 3)
            {
                throw new EPathException($"Logical Segment Type({readingType}) is not supported");
            }

        }

        public void WriteValue(DataProcessor processor)
        {
            var type = this.Type;
            var mod = type % 4;
            var value = this.Value;

            if (mod == 0)
            {
                processor.WriteByte((byte)value);
            }
            else if (mod == 1)
            {
                // Pad to words
                processor.WriteByte(0);
                processor.WriteUShort((ushort)value);
            }
            else if (mod == 2)
            {
                // Pad to words
                processor.WriteByte(0);
                processor.WriteUInt(value);
            }
            else if (mod == 3)
            {
                throw new EPathException($"Logical Segment Type({type}) is not supported");
            }

        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.TypeBase.GetHashCode();
            hash = hash * 31 + this.Value.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            return obj is EPathSegmentLogical other && this.Equals(other);
        }

        public bool Equals(EPathSegmentLogical other)
        {
            if (this.GetType().Equals(other.GetType()) == false)
            {
                return false;
            }

            if (this.TypeBase != other.TypeBase)
            {
                return false;
            }

            if (this.Value != other.Value)
            {
                return false;
            }

            return true;
        }

    }

}
