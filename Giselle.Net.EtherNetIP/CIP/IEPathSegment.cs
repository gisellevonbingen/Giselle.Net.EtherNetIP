using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP.CIP
{
    public interface IEPathSegment
    {
        byte TypeAssembly { get; }

        void ReadValue(byte readingTypeAssembly, DataProcessor processor);

        void WriteValue(DataProcessor processor);

    }

}
