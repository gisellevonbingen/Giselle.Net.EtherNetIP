using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class CommandItemRegistration
    {
        public ushort Id { get; private set; }
        public Type RequestType { get; private set; }
        public Type ResponseType { get; private set; }

        public CommandItemRegistration(ushort id, Type requestType, Type responseType)
        {
            this.Id = id;
            this.RequestType = requestType;
            this.ResponseType = responseType;
        }

        public Type SelectType(bool isRequest) { return isRequest ? this.RequestType : this.ResponseType; }

    }

}
