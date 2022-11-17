using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP.CIP
{
    public interface IEPathSegment
    {
        byte Type { get; }

        void ReadValue(byte readingType, DataProcessor processor);

        void WriteValue(DataProcessor processor);

    }

}
