using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    public static class IPAddressUtils
    {
        public static bool IsIPv4(this IPAddress address)
        {
            return address != null && address.IsIPv6LinkLocal == false && address.IsIPv6Multicast == false;
        }

        public static IPAddress ToIPv4Address(this int address, bool isBigEndian = true)
        {
            var bytes = new byte[4];

            if (isBigEndian == true)
            {
                bytes[0] = (byte)((address >> 24) & 0xFF);
                bytes[1] = (byte)((address >> 16) & 0xFF);
                bytes[2] = (byte)((address >> 08) & 0xFF);
                bytes[3] = (byte)((address >> 00) & 0xFF);
            }
            else
            {
                bytes[0] = (byte)((address >> 00) & 0xFF);
                bytes[1] = (byte)((address >> 08) & 0xFF);
                bytes[2] = (byte)((address >> 16) & 0xFF);
                bytes[3] = (byte)((address >> 24) & 0xFF);
            }

            return new IPAddress(bytes);
        }

        public static int ToIPv4Address(this IPAddress address, bool isBigEndian = true)
        {
            var bytes = address.MapToIPv4().GetAddressBytes();
            int value = 0;

            if (isBigEndian == true)
            {
                value |= bytes[0] << 24;
                value |= bytes[1] << 16;
                value |= bytes[2] << 08;
                value |= bytes[3] << 00;
            }
            else
            {
                value |= bytes[0] << 00;
                value |= bytes[1] << 08;
                value |= bytes[2] << 16;
                value |= bytes[3] << 24;
            }

            return value;
        }

        public static bool IsSameSubnet(this IPAddress value, IPAddress subnetMask, IPAddress other)
        {
            var network1 = value.GetNetworkAddress(subnetMask);
            var network2 = other.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }

        public static IPAddress GetNetworkAddress(this IPAddress value, IPAddress subnetMask)
        {
            var valueBytes = value.GetAddressBytes();
            var subnetBytes = subnetMask.GetAddressBytes();

            if (valueBytes.Length != subnetBytes.Length)
            {
                throw new ArgumentException("value and subnetmask's length not matched");
            }

            var networkBytes = new byte[valueBytes.Length];

            for (var i = 0; i < networkBytes.Length; i++)
            {
                networkBytes[i] = (byte)(valueBytes[i] & subnetBytes[i]);
            }

            return new IPAddress(networkBytes);
        }

    }

}
