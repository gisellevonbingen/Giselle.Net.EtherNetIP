using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    [Serializable]
    public class PathSegmentException : Exception
    {
        public PathSegmentException() { }
        public PathSegmentException(string message) : base(message) { }
        public PathSegmentException(string message, Exception inner) : base(message, inner) { }
        protected PathSegmentException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }

}
