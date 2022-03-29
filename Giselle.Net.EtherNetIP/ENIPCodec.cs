using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Giselle.Net.EtherNetIP
{
    public class ENIPCodec
    {
        public const int ConnectionIDMax = 0xFFFFFFF;
        public const int ConnectionSerialNumberMax = 0xFFFF;

        public uint SessionID { get; set; }
        public Random Random { get; set; }

        public ENIPCodec()
        {
            this.SessionID = 0;
            this.Random = new Random();
        }

        public IdentifyObject CreateIdentifyObject(Stream stream) { return new IdentifyObject(this, stream); }

        public void WriteEncapsulation(Stream stream, Encapsulation request)
        {
            using (var ms = new MemoryStream())
            {
                var processor = new ENIPProcessor(ms);
                processor.WriteEncapsulation(request);

                var bytes = ms.ToArray();
                stream.Write(bytes, 0, bytes.Length);
            }

        }

        public Encapsulation ReadEncapsulation(Stream stream)
        {
            return new ENIPProcessor(stream).ReadEncapsulation();
        }

        public Encapsulation CreateEncapsulation()
        {
            var encapsulation = new Encapsulation();
            encapsulation.SessionID = this.SessionID;

            return encapsulation;
        }

        public Encapsulation CreateSendRRData(params CommandItem[] collection)
        {
            return this.CreateSendRRData((IEnumerable<CommandItem>)collection);
        }

        public Encapsulation CreateSendRRData(IEnumerable<CommandItem> collection)
        {
            var data = new CommandData();
            data.Items.Add(new CommandItemNull());
            data.Items.AddRange(collection);
            return this.CreateSendRRData(data);
        }

        public Encapsulation CreateSendRRData(CommandData data)
        {
            var encapsulation = this.CreateEncapsulation();
            encapsulation.Command = EncapsulationCommand.SendRRData;
            data.Write(encapsulation.DataProcessor);

            return encapsulation;
        }

        public Encapsulation CreateRegisterSession()
        {
            var request = new Encapsulation();
            request.Command = EncapsulationCommand.RegisterSession;
            request.DataProcessor.WriteUInt(1);
            request.SessionID = 0;
            return request;
        }

        public uint RegisterSession(Stream stream)
        {
            var sessionId = this.SessionID;

            if (sessionId > 0)
            {
                return sessionId;
            }
            else
            {
                var request = this.CreateRegisterSession();
                this.WriteEncapsulation(stream, request);

                var response = this.ReadEncapsulation(stream);
                return this.HandleRegisterSession(response);
            }

        }

        public uint HandleRegisterSession(Encapsulation response)
        {
            this.SessionID = response.SessionID;
            return response.SessionID;
        }

        public Encapsulation CreateUnRegisterSession()
        {
            var request = this.CreateEncapsulation();
            request.Command = EncapsulationCommand.UnRegisterSession;
            return request;
        }

        public void HandleUnRegisterSession()
        {
            this.SessionID = 0;
        }

        public void UnRegisterSession(Stream stream)
        {
            var request = this.CreateUnRegisterSession();
            this.WriteEncapsulation(stream, request);

            this.HandleUnRegisterSession();
        }

        public Encapsulation CreateGetAttribute(RequestPath path)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.Command = (byte)(path.AttributeID == 0 ? CommonPacketCommand.GetAttributeAll : CommonPacketCommand.GetAttributeSingle);
            udRequest.Path = path;

            return this.CreateSendRRData(udRequest);
        }

        public ENIPProcessor HandleGetAttribute(Encapsulation response)
        {
            var responseData = response.DataProcessor.ReadCommandData();
            return responseData.Items.Find<CommandItemUnconnectedDataResponse>().DataProcessor;
        }

        public ENIPProcessor GetAttribute(Stream stream, RequestPath path)
        {
            var request = this.CreateGetAttribute(path);
            this.WriteEncapsulation(stream, request);

            var response = this.ReadEncapsulation(stream);
            return this.HandleGetAttribute(response);
        }

        public Encapsulation CreateSetAttribute(RequestPath path, byte[] values)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.Command = (byte)CommonPacketCommand.SetAttribute;
            udRequest.Path = path;
            udRequest.DataProcessor.WriteBytes(values);

            return this.CreateSendRRData(udRequest);
        }

        public ENIPProcessor HandleSetAttribute(Encapsulation response)
        {
            var responseData = response.DataProcessor.ReadCommandData();
            return responseData.Items.Find<CommandItemUnconnectedDataResponse>().DataProcessor;
        }

        public ENIPProcessor SetAttribute(Stream stream, RequestPath path, byte[] values)
        {
            var request = this.CreateSetAttribute(path, values);
            this.WriteEncapsulation(stream, request);

            var response = this.ReadEncapsulation(stream);
            return this.HandleSetAttribute(response);
        }

        public Encapsulation CreateForwardOpen(ForwardOpenOptions options)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.Command = (byte)CommonPacketCommand.ForwardOpen;
            udRequest.Path = new RequestPath(KnownClassID.ConnectionManager, 0x01);

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

            var pathTuples = new ConnectionPath
            {
                new PathSegment(RequestPath.ClassBase, options.ClassID),
                new PathSegment(RequestPath.InstanceBase, 0x01)
            };

            if (options.O_T_Assembly.ConnectionType != ConnectionType.Null)
            {
                pathTuples.Add(new PathSegment(RequestPath.ConnectionPointBase, options.O_T_Assembly.InstanceID));
            }

            if (options.T_O_Assembly.ConnectionType != ConnectionType.Null)
            {
                pathTuples.Add(new PathSegment(RequestPath.ConnectionPointBase, options.T_O_Assembly.InstanceID));
            }

            pathTuples.Write(reqProcessor);

            var requestIPV4EndPoint = new CommandItemEndPoint_T_O
            {
                EndPoint = new IPv4EndPoint() { Family = 2, Port = options.OriginatorUDPPort }
            };

            if (options.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                requestIPV4EndPoint.EndPoint.Address = ((int)GetMulticastAddress((uint)options.LocalAddress.ToIPv4Address())).ToIPv4Address(true);
            }
            else
            {
                requestIPV4EndPoint.EndPoint.Address = IPAddress.Any;
            }

            return this.CreateSendRRData(udRequest, requestIPV4EndPoint);
        }

        public ForwardOpenResult HandleForwardOpen(Encapsulation response, ForwardOpenOptions options)
        {
            var commandData = response.DataProcessor.ReadCommandData();
            var result = new ForwardOpenResult() { Options = options };

            foreach (var item in commandData.Items)
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

        public ForwardOpenResult ForwardOpen(Stream stream, ForwardOpenOptions options)
        {
            var request = this.CreateForwardOpen(options);
            this.WriteEncapsulation(stream, request);

            var response = this.ReadEncapsulation(stream);
            return this.HandleForwardOpen(response, options);
        }

        public Encapsulation CreateForwardClose(ForwardCloseOptions options)
        {
            var udRequest = new CommandItemUnconnectedDataRequest();
            udRequest.Command = (byte)CommonPacketCommand.ForwardClose;
            udRequest.Path = new RequestPath(KnownClassID.ConnectionManager, 0x01);

            var reqProcessor = udRequest.DataProcessor;
            reqProcessor.WriteByte(options.TickTime);
            reqProcessor.WriteByte(options.TimeoutTicks);

            reqProcessor.WriteUShort(options.ConnectionSerialNumber);
            reqProcessor.WriteUShort(options.OriginatorVenderID);
            reqProcessor.WriteUInt(options.OriginatorSerialNumber);

            var connectionPath = new ConnectionPath
            {
               new PathSegment(RequestPath.ClassBase, options.ClassID),
               new PathSegment(RequestPath.InstanceBase, 0x01),
            };
            connectionPath.HasReserved = true;

            if (options.O_T_ConnectionType != ConnectionType.Null)
            {
                connectionPath.Add(new PathSegment(RequestPath.ConnectionPointBase, options.O_T_InstanceID));
            }

            if (options.T_O_ConnectionType != ConnectionType.Null)
            {
                connectionPath.Add(new PathSegment(RequestPath.ConnectionPointBase, options.T_O_InstanceID));
            }

            connectionPath.Write(reqProcessor);

            return this.CreateSendRRData(udRequest);
        }

        public ForwardCloseResult HandleForwardClose(Encapsulation response, ForwardCloseOptions options)
        {
            var commandData = response.DataProcessor.ReadCommandData();
            var result = new ForwardCloseResult() { Options = options };

            foreach (var item in commandData.Items)
            {
                if (item is CommandItemUnconnectedDataResponse udResponse)
                {
                    result.Error = udResponse.Error;
                    result.ExtendedStatus = udResponse.ExtendedStatus.Length > 0 ? (ExtendedStatusError)udResponse.ExtendedStatus[0] : ExtendedStatusError.Success;
                }

            }

            return result;
        }

        public ForwardCloseResult ForwardClose(Stream stream, ForwardCloseOptions options)
        {
            var request = this.CreateForwardClose(options);
            this.WriteEncapsulation(stream, request);

            var response = this.ReadEncapsulation(stream);
            return this.HandleForwardClose(response, options);
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
