﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giselle.Net.EtherNetIP.CIP;
using Giselle.Net.EtherNetIP.ENIP;

namespace Giselle.Net.EtherNetIP
{
    public static class DataProcessorExtensions
    {
        public static EPath ReadEPath(this DataProcessor processor) => new EPath(processor);

        public static void WriteEPath(this DataProcessor processor, EPath epath) => epath.Write(processor);

        public static SendRRData ReadCommandData(this DataProcessor processor, bool isRequest) => new SendRRData(processor, isRequest);

        public static void WriteCommandData(this DataProcessor processor, SendRRData commandData) => commandData.Write(processor);

        public static CommandItems ReadCommandItems(this DataProcessor processor, bool isRequest) => new CommandItems(processor, isRequest);

        public static void WriteCommnadItems(this DataProcessor processor, CommandItems items) => items.Write(processor);

        public static Encapsulation ReadEncapsulation(this DataProcessor processor) => new Encapsulation(processor);

        public static void WriteEncapsulation(this DataProcessor processor, Encapsulation encapsulation) => encapsulation.Write(processor);

    }

}
