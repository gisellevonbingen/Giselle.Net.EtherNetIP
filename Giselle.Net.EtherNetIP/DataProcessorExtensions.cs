using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    public static class DataProcessorExtensions
    {
        public static Encapsulation ReadEncapsulation(this DataProcessor processor) => new Encapsulation(processor);

        public static void WriteEncapsulation(this DataProcessor processor, Encapsulation encapsulation) => encapsulation.Write(processor);

        public static CommandItems ReadCPacket(this DataProcessor processor, bool isRequest = false) => new CommandItems(processor, isRequest);

        public static void WriteCommonPacket(this DataProcessor processor, CommandItems packet) => packet.Write(processor);

        public static PathSegments ReadPathSegments(this DataProcessor processor) => new PathSegments(processor);

        public static void WritePathSegments(this DataProcessor processor, PathSegments epath) => epath.Write(processor);

        public static CommandData ReadCommandData(this DataProcessor processor, bool isRequest = false) => new CommandData(processor, isRequest);

        public static void WriteCommandData(this DataProcessor processor, CommandData epath) => epath.Write(processor);

    }

}
