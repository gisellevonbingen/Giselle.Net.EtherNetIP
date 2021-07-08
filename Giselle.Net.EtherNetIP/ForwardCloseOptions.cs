using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class ForwardCloseOptions : AbstractOptions
    {
        public ConnectionType O_T_ConnectionType { get; private set; }
        public ushort O_T_InstanceID { get; private set; }

        public ConnectionType T_O_ConnectionType { get; private set; }
        public ushort T_O_InstanceID { get; private set; }

        public ForwardCloseOptions()
        {
            this.O_T_ConnectionType = ConnectionType.Null;
            this.O_T_InstanceID = KnownAssemblyInstanceID.PrimaryOutput;

            this.T_O_ConnectionType = ConnectionType.Null;
            this.T_O_InstanceID = KnownAssemblyInstanceID.PrimaryInput;
        }

        public ForwardCloseOptions(ForwardCloseOptions other)
            : base(other)
        {
            this.O_T_ConnectionType = other.O_T_ConnectionType;
            this.O_T_InstanceID = other.O_T_InstanceID;

            this.T_O_ConnectionType = other.T_O_ConnectionType;
            this.T_O_InstanceID = other.T_O_InstanceID;
        }

        public ForwardCloseOptions(ForwardOpenResult openResult)
            : base(openResult.Options)
        {
            this.ConnectionSerialNumber = openResult.ConnectionSerialNumber;

            var openOptions = openResult.Options;

            this.O_T_ConnectionType = openOptions.O_T_Assembly.ConnectionType;
            this.O_T_InstanceID = openOptions.O_T_Assembly.InstanceID;

            this.T_O_ConnectionType = openOptions.T_O_Assembly.ConnectionType;
            this.T_O_InstanceID = openOptions.T_O_Assembly.InstanceID;
        }

    }

}
