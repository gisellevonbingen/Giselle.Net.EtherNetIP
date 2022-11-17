using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public abstract class CommandItem
    {
        public CommandItem()
        {

        }

        public virtual void Read(DataProcessor processor)
        {

        }

        public virtual void Write(DataProcessor processor)
        {

        }

    }

}
