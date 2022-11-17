using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP.CIP
{
    [Serializable]
    public class EPathException : Exception
    {
        public EPathException() { }
        public EPathException(string message) : base(message) { }
        public EPathException(string message, Exception inner) : base(message, inner) { }
        protected EPathException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }

}
