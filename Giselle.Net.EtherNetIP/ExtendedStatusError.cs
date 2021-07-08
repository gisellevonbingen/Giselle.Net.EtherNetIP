using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public enum ExtendedStatusError : ushort
    {
        Success = 0x0000,
        DuplicateForwardOpen = 0x0100,
        TriggerCombinationNotSupported = 0x0103,
        OwnerConflict = 0x0106,
        TargetConnectionNotFound = 0x0107,
        RpiNotSupported = 0x0111,
        VendorIdOrProductCodeMismatch = 0x0114,
        DeviceTypeMismatch = 0x0115,
        RevisionMismatch = 0x0116,
        TargetObjectOutOfConnections = 0x011A,
        InvalidOriginatorToTargetNetworkConnectionType = 0x0123,
        InvalidTargetToOriginatorNetworkConnectionType = 0x0124,
        InvalidOriginatorToTargetSize = 0x0127,
        InvalidTargetToOriginatorSize = 0x0128,
        InvalidConsumingApplicationPath = 0x012A,
        InvalidProducingApplicationPath = 0x012B,
        InconsistentApplicationPathCombination = 0x012F,
        NullForwardOpenFunctionNotSupported = 0x0132,
        InvalidSegmentInConnectionPath = 0x0315,
        NotConfigurableForOffSubnetMulticast = 0x0813,
    }

}
