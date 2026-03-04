using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class ForwardCloseOptions : AbstractOptions
    {
        public ConnectionType O_T_ConnectionType { get; private set; }
        public ushort O_T_InstanceId { get; private set; }

        public ConnectionType T_O_ConnectionType { get; private set; }
        public ushort T_O_InstanceId { get; private set; }

        public ForwardCloseOptions()
        {
            this.O_T_ConnectionType = ConnectionType.Null;
            this.T_O_ConnectionType = ConnectionType.Null;
        }

        public ForwardCloseOptions(ForwardCloseOptions other)
            : base(other)
        {
            this.O_T_ConnectionType = other.O_T_ConnectionType;
            this.O_T_InstanceId = other.O_T_InstanceId;

            this.T_O_ConnectionType = other.T_O_ConnectionType;
            this.T_O_InstanceId = other.T_O_InstanceId;
        }

        public ForwardCloseOptions(ForwardOpenOptions openOptions, ForwardOpenResult openResult)
        {
            this.ConnectionSerialNumber = openResult.ConnectionSerialNumber;

            this.O_T_ConnectionType = openOptions.O_T_Assembly.ConnectionType;
            this.O_T_InstanceId = openOptions.O_T_Assembly.InstanceId;

            this.T_O_ConnectionType = openOptions.T_O_Assembly.ConnectionType;
            this.T_O_InstanceId = openOptions.T_O_Assembly.InstanceId;
        }

    }

}
