using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public struct PathSegment
    {
        public byte Base { get; set; }
        public ushort ID { get; set; }

        public PathSegment(byte @base, ushort id = 0)
            : this()
        {
            this.Base = @base;
            this.ID = id;
        }

        public void Read(ENIPProcessor processor)
        {
            var offsetBase = processor.ReadByte();
            var mod = offsetBase % 2;
            this.Base = (byte)(offsetBase - mod);

            if (mod == 0)
            {
                this.ID = processor.ReadByte();
            }
            else if (mod == 1)
            {
                this.ID = processor.ReadUShort();
            }

        }

        public void Write(ENIPProcessor processor)
        {
            var id = this.ID;
            var @base = this.Base;

            if (id < byte.MaxValue)
            {
                processor.WriteByte((byte)(@base + 0));
                processor.WriteByte((byte)id);
            }
            else
            {
                processor.WriteByte((byte)(@base + 1));
                processor.WriteUShort(id);
            }

        }

    }

}
