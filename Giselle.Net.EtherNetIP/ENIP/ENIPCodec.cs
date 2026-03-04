using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class ENIPCodec : CIPCodec
    {
        public uint SessionId { get; set; }
        public ushort SendRRDataTimeout { get; set; }

        public ENIPCodec()
        {
            this.SessionId = 0;
            this.SendRRDataTimeout = 0;
        }

        public Encapsulation CreateEncapsulation()
        {
            var encapsulation = new Encapsulation();
            encapsulation.SessionId = this.SessionId;

            return encapsulation;
        }

        public Encapsulation CreateEncapsulation(SendRRData data)
        {
            var encapsulation = this.CreateEncapsulation();
            encapsulation.Command = EncapsulationCommand.SendRRData;
            data.Write(encapsulation.DataProcessor);

            return encapsulation;
        }

        public SendRRData ReadCommandData(Encapsulation response, bool isRequest) => new SendRRData(response.DataProcessor, isRequest);

        public Encapsulation CreateRegisterSession()
        {
            var request = new Encapsulation();
            request.Command = EncapsulationCommand.RegisterSession;
            request.DataProcessor.WriteUInt(1);
            request.SessionId = 0;
            return request;
        }

        public uint HandleRegisterSession(Encapsulation response)
        {
            this.SessionId = response.SessionId;
            return response.SessionId;
        }

        public Encapsulation CreateUnRegisterSession()
        {
            var request = this.CreateEncapsulation();
            request.Command = EncapsulationCommand.UnRegisterSession;
            return request;
        }

        public void HandleUnRegisterSession()
        {
            this.SessionId = 0;
        }

        public SendRRData CreateSendRRData(params CommandItem[] items) => this.CreateSendRRData((IEnumerable<CommandItem>)items);

        public SendRRData CreateSendRRData(IEnumerable<CommandItem> items)
        {
            var sendRRData = new SendRRData() { Timeout = this.SendRRDataTimeout };
            sendRRData.Items.Add(new CommandItemNull());
            sendRRData.Items.AddRange(items);
            return sendRRData;
        }

        public UdpClient CreateImplictMessagingClient(ForwardOpenOptions options, ForwardOpenResult result, IPAddress localAddress)
        {
            var udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(localAddress, options.T_O_UDPPort));
            JoinMulticastGroup(udpClient, options, result, localAddress);

            return udpClient;
        }

        public static void JoinMulticastGroup(UdpClient udpClient, ForwardOpenOptions options, ForwardOpenResult result, IPAddress localAddress)
        {
            if (options.O_T_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                udpClient.JoinMulticastGroup(result.O_T_Address.Address, localAddress);
            }

            if (options.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                udpClient.JoinMulticastGroup(result.T_O_Address.Address, localAddress);
            }

        }

    }

}
