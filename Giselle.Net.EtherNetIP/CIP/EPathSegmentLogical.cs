using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public struct EPathSegmentLogical : IEquatable<EPathSegmentLogical>, IEPathSegment
    {
        public const byte SegmentType = 0x01;

        public const byte LogicalTypeMask = 0x1C;
        public const byte LogicalTypeOffset = 2;
        public const byte FormatTypeMask = 0x03;
        public const byte FormatTypeOffset = 0;

        public const byte FormatType8Bits = 0x00;
        public const byte FormatType16Bits = 0x01;
        public const byte FormatType32Bits = 0x02;

        public static byte ToLogicalType(byte typeAssembly) => (byte)((typeAssembly & LogicalTypeMask) >> LogicalTypeOffset);

        public static byte ToFormatType(byte typeAssembly) => (byte)((typeAssembly & FormatTypeMask) >> FormatTypeOffset);

        public static EPathSegmentLogical FromClassID(uint value) => new EPathSegmentLogical(KnownEPathLogicalSegmentType.ClassID, value);

        public static EPathSegmentLogical FromInstanceID(uint value) => new EPathSegmentLogical(KnownEPathLogicalSegmentType.InstanceID, value);

        public static EPathSegmentLogical FromElementID(uint value) => new EPathSegmentLogical(KnownEPathLogicalSegmentType.ElementID, value);

        public static EPathSegmentLogical FromConnectionPointID(uint value) => new EPathSegmentLogical(KnownEPathLogicalSegmentType.ConnectionPointID, value);

        public static EPathSegmentLogical FromAttributeID(uint value) => new EPathSegmentLogical(KnownEPathLogicalSegmentType.AttributeID, value);

        public byte LogicalType { get; set; }
        public uint Value { get; set; }

        public EPathSegmentLogical(byte logicalType, uint value = 0)
            : this()
        {
            this.LogicalType = logicalType;
            this.Value = value;
        }

        public byte TypeAssembly
        {
            get
            {
                return EPath.MergeSegmentTypeAssembly(SegmentType, (byte)(
                    (LogicalTypeMask & (this.LogicalType << LogicalTypeOffset)) |
                    (FormatTypeMask & (this.FormatType << FormatTypeOffset))));
            }

        }

        public byte FormatType
        {
            get
            {
                var id = this.Value;

                if (id > ushort.MaxValue)
                {
                    return FormatType32Bits;
                }
                else if (id > byte.MaxValue)
                {
                    return FormatType16Bits;
                }
                else
                {
                    return FormatType8Bits;
                }

            }

        }

        public void ReadValue(byte readingTypeAssembly, DataProcessor processor)
        {
            var formatType = ToFormatType(readingTypeAssembly);

            if (formatType == FormatType8Bits)
            {
                this.Value = processor.ReadByte();
            }
            else if (formatType == FormatType16Bits)
            {
                // Pad to words
                processor.ReadByte();
                this.Value = processor.ReadUShort();
            }
            else if (formatType == FormatType32Bits)
            {
                // Pad to words
                processor.ReadByte();
                this.Value = processor.ReadUInt();
            }
            else
            {
                throw new EPathException($"Unknown Logical Segment FormatType: ({formatType:X2})");
            }

        }

        public void WriteValue(DataProcessor processor)
        {
            var formatType = this.FormatType;
            var value = this.Value;

            if (formatType == FormatType8Bits)
            {
                processor.WriteByte((byte)value);
            }
            else if (formatType == FormatType16Bits)
            {
                // Pad to words
                processor.WriteByte(0);
                processor.WriteUShort((ushort)value);
            }
            else if (formatType == FormatType32Bits)
            {
                // Pad to words
                processor.WriteByte(0);
                processor.WriteUInt(value);
            }
            else
            {
                throw new EPathException($"Unknown Logical Segment FormatType: ({formatType:X2})");
            }

        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.LogicalType.GetHashCode();
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

            if (this.LogicalType != other.LogicalType)
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
