using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public abstract class CommandItem
    {
        public CommandItem()
        {

        }

        public virtual void Read(ENIPProcessor processor)
        {

        }

        public virtual void Write(ENIPProcessor processor)
        {

        }

    }

}
