using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class ConnectionPath : List<PathSegment>
    {
        public bool HasReserved { get; set; }
        public byte Reserved { get; set; }

        public ConnectionPath()
        {

        }

        public void Read(DataProcessor processor)
        {
            var count = processor.ReadByte();

            if (this.HasReserved == true)
            {
                this.Reserved = processor.ReadByte();
            }

            this.Clear();

            for (var i = 0; i < count; i++)
            {
                var tuple = new PathSegment();
                tuple.Read(processor);
                this.Add(tuple);
            }

        }

        public void Write(DataProcessor processor)
        {
            processor.WriteByte((byte)this.Count);

            if (this.HasReserved == true)
            {
                processor.WriteByte(this.Reserved);
            }

            foreach (var tuple in this)
            {
                tuple.Write(processor);
            }

        }

    }

}
