using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP.CIP
{
    public class CIPCodec
    {
        public const int ConnectionIDMax = 0xFFFFFFF;
        public const int ConnectionSerialNumberMax = 0xFFFF;

        public static DataProcessor CreateDataProcessor(Stream baseStream) => new DataProcessor(baseStream) { IsLittleEndian = true };

        public Random Random { get; set; }

        public CIPCodec()
        {
            this.Random = new Random();
        }

        public CommandItemUnconnectedDataRequest CreateGetAttribute(AttributePath path)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.ServiceCode = path.AttributeID == 0 ? ServiceCode.GetAttributeAll : ServiceCode.GetAttributeSingle;
            udRequest.Path = path.AsEPath();

            return udRequest;
        }

        public DataProcessor HandleGetAttribute(CommandItems response)
        {
            return response.Find<CommandItemUnconnectedDataResponse>().DataProcessor;
        }

        public CommandItemUnconnectedDataRequest CreateSetAttribute(AttributePath path, byte[] values)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.ServiceCode = ServiceCode.SetAttributeSingle;
            udRequest.Path = path.AsEPath();
            udRequest.DataProcessor.WriteBytes(values);

            return udRequest;
        }

        public DataProcessor HandleSetAttribute(CommandItems response)
        {
            return response.Find<CommandItemUnconnectedDataResponse>().DataProcessor;
        }

        public CommandItem CreateForwardOpen(ForwardOpenOptions options)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.ServiceCode = ServiceCode.ForwardOpen;
            udRequest.Path = new EPath()
            {
                EPathSegmentLogical.FromClassID(KnownClassID.ConnectionManager),
                EPathSegmentLogical.FromInstanceID(0x01)
            };

            var reqProcessor = udRequest.DataProcessor;
            reqProcessor.WriteByte(options.TickTime);
            reqProcessor.WriteByte(options.TimeoutTicks);

            var otConnectionIDReq = options.O_T_Assembly.ConnectionID != 0 ? options.O_T_Assembly.ConnectionID : (ushort)(this.NextConnectionID() + 0);
            var toConnectionIDReq = options.T_O_Assembly.ConnectionID != 0 ? options.T_O_Assembly.ConnectionID : (ushort)(this.NextConnectionID() + 1);
            reqProcessor.WriteUInt(otConnectionIDReq);
            reqProcessor.WriteUInt(toConnectionIDReq);

            var connectionSerialNumber = options.ConnectionSerialNumber != 0 ? options.ConnectionSerialNumber : (ushort)(this.NextConnectionSerialNumber() + 2);
            reqProcessor.WriteUShort(connectionSerialNumber);
            reqProcessor.WriteUShort(options.OriginatorVenderID);
            reqProcessor.WriteUInt(options.OriginatorSerialNumber);
            reqProcessor.WriteByte(options.TimeoutMultiplier);

            // Reserved
            reqProcessor.WriteByte(0x00);
            reqProcessor.WriteByte(0x00);
            reqProcessor.WriteByte(0x00);

            reqProcessor.WriteUInt(options.O_T_Assembly.RequestPacketRate);
            reqProcessor.WriteUShort(options.O_T_Assembly.Flags);
            reqProcessor.WriteUInt(options.T_O_Assembly.RequestPacketRate);
            reqProcessor.WriteUShort(options.T_O_Assembly.Flags);

            // Transport Type/Trigger
            reqProcessor.WriteByte(0x01);

            var connectionPath = new EPath
            {
                EPathSegmentLogical.FromClassID(options.ClassID),
                EPathSegmentLogical.FromInstanceID(0x01)
            };

            if (options.O_T_Assembly.ConnectionType != ConnectionType.Null)
            {
                connectionPath.Add(EPathSegmentLogical.FromConnectionPointID(options.O_T_Assembly.InstanceID));
            }

            if (options.T_O_Assembly.ConnectionType != ConnectionType.Null)
            {
                connectionPath.Add(EPathSegmentLogical.FromConnectionPointID(options.T_O_Assembly.InstanceID));
            }

            connectionPath.Write(reqProcessor);

            var requestIPV4EndPoint = new CommandItemEndPoint_T_O
            {
                EndPoint = new IPv4EndPoint() { Family = 2, Port = options.T_O_UDPPort }
            };

            if (options.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                requestIPV4EndPoint.EndPoint.Address = ((int)GetMulticastAddress((uint)options.LocalAddress.ToIPv4Address())).ToIPv4Address(true);
            }
            else
            {
                requestIPV4EndPoint.EndPoint.Address = IPAddress.Any;
            }

            return udRequest;
        }

        public ForwardOpenResult HandleForwardOpen(CommandItems response, ForwardOpenOptions options)
        {
            var result = new ForwardOpenResult() { Options = options };

            foreach (var item in response)
            {
                if (item is CommandItemUnconnectedDataResponse udResponse)
                {
                    result.Error = udResponse.Error;
                    result.ExtendedStatus = udResponse.ExtendedStatus.Length > 0 ? (ExtendedStatusError)udResponse.ExtendedStatus[0] : ExtendedStatusError.Success;

                    var resProcessor = udResponse.DataProcessor;

                    if (udResponse.Error == 0)
                    {
                        result.O_T_ConnectionID = resProcessor.ReadUInt();
                        result.T_O_ConnectionID = resProcessor.ReadUInt();
                        result.ConnectionSerialNumber = resProcessor.ReadUShort();
                    }

                }
                else if (item is CommandItemEndPoint_O_T t)
                {
                    result.O_T_Address = t.EndPoint;
                }
                else if (item is CommandItemEndPoint_T_O o)
                {
                    result.T_O_Address = o.EndPoint;
                }

            }

            return result;
        }

        public CommandItemUnconnectedDataRequest CreateForwardClose(ForwardCloseOptions options)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.ServiceCode = ServiceCode.ForwardClose;
            udRequest.Path = new EPath()
            {
                EPathSegmentLogical.FromClassID(KnownClassID.ConnectionManager),
                EPathSegmentLogical.FromInstanceID(0x01)
            };

            var reqProcessor = udRequest.DataProcessor;
            reqProcessor.WriteByte(options.TickTime);
            reqProcessor.WriteByte(options.TimeoutTicks);

            reqProcessor.WriteUShort(options.ConnectionSerialNumber);
            reqProcessor.WriteUShort(options.OriginatorVenderID);
            reqProcessor.WriteUInt(options.OriginatorSerialNumber);

            var connectionPath = new EPath
            {
               EPathSegmentLogical.FromClassID(options.ClassID),
               EPathSegmentLogical.FromInstanceID(0x01),
            };
            connectionPath.HasReserved = true;

            if (options.O_T_ConnectionType != ConnectionType.Null)
            {
                connectionPath.Add(EPathSegmentLogical.FromConnectionPointID(options.O_T_InstanceID));
            }

            if (options.T_O_ConnectionType != ConnectionType.Null)
            {
                connectionPath.Add(EPathSegmentLogical.FromConnectionPointID(options.T_O_InstanceID));
            }

            connectionPath.Write(reqProcessor);

            return udRequest;
        }

        public ForwardCloseResult HandleForwardClose(CommandItems response, ForwardCloseOptions options)
        {
            var result = new ForwardCloseResult() { Options = options };

            foreach (var item in response)
            {
                if (item is CommandItemUnconnectedDataResponse udResponse)
                {
                    result.Error = udResponse.Error;
                    result.ExtendedStatus = udResponse.ExtendedStatus.Length > 0 ? (ExtendedStatusError)udResponse.ExtendedStatus[0] : ExtendedStatusError.Success;
                }

            }

            return result;
        }

        public CommandItemSequencedAddress HandleImplicitTransmission(RealTimeFormat realTimeFormat, CommandItems items, byte[] buffer)
        {
            CommandItemSequencedAddress sequencedAddress = null;

            foreach (var item in items)
            {
                if (item is CommandItemSequencedAddress address)
                {
                    sequencedAddress = address;
                }
                else if (item is CommandItemConnectedData cd)
                {
                    var processor = cd.DataProcessor;

                    if (realTimeFormat != RealTimeFormat.Heartbeat)
                    {
                        processor.ReadUShort();
                    }
                    else
                    {
                        processor.ReadUShort();
                    }

                    if (realTimeFormat == RealTimeFormat.Header32Bit)
                    {
                        processor.ReadUInt();
                    }

                    processor.ReadBytes(buffer);
                }

            }

            return sequencedAddress;
        }

        public CommandItems CreateImplicitTransmission(RealTimeFormat realTimeFormat, CommandItemSequencedAddress sequencedAddress, byte[] buffer)
        {
            var cd = new CommandItemConnectedData();
            var processor = cd.DataProcessor;

            if (realTimeFormat != RealTimeFormat.Heartbeat)
            {
                processor.WriteUShort((ushort)sequencedAddress.SequenceCount);
            }
            else
            {
                processor.WriteUShort(0);
            }

            if (realTimeFormat == RealTimeFormat.Header32Bit)
            {
                processor.WriteUInt(1);
            }

            processor.WriteBytes(buffer);

            return new CommandItems
            {
                sequencedAddress,
                cd
            };

        }

        private static uint GetMulticastAddress(uint deviceIPAddress)
        {
            uint num = 0xEFC00100;
            uint num2 = 0x03FF;
            uint num3 = 0;

            if (deviceIPAddress <= 0x7FFFFFFF)
            {
                num3 = 0xFF000000;
            }
            else if (deviceIPAddress >= 0x80000000 && deviceIPAddress <= 0xBFFFFFFF)
            {
                num3 = 0xFFFF0000;
            }
            else if (deviceIPAddress >= 0xC0000000 && deviceIPAddress <= 0xDFFFFFFF)
            {
                num3 = 0xFFFFFF00;
            }

            uint num4 = deviceIPAddress & ~num3;
            uint num5 = num4 - 1;
            num5 &= num2;
            return num + num5 * 32;
        }

        public int NextConnectionID()
        {
            return this.Random.Next(ConnectionIDMax);
        }

        public int NextConnectionSerialNumber()
        {
            return this.Random.Next(ConnectionSerialNumberMax);
        }

    }

}
