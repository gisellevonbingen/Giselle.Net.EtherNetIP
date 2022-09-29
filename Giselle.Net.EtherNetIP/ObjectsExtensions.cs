using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    internal static class ObjectsExtensions
    {
        public static void InterruptQuietly(this Thread thraed)
        {
            if (thraed != null)
            {
                try
                {
                    thraed.Interrupt();
                }
                catch
                {

                }

            }

        }

        public static void DisposeQuietly(this IDisposable disposable)
        {
            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                }
                catch
                {

                }

            }

        }

    }

}
