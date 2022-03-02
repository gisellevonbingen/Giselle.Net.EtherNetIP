using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP.Test
{
    public class Program
    {
        public static void Main()
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect("192.168.1.200", 0xAF12);

                var stream = tcpClient.GetStream();
                var localAddress = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address;
                var remoteAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                var codec = new ENIPCodec();

                try
                {
                    Identify(stream, codec);
                    Console.WriteLine();
                    Console.WriteLine("Enter to ForwardOpen");
                    Console.ReadLine();

                    codec.RegisterSession(stream);
                    var openResult = ForwardOpen(stream, codec, localAddress);

                    if (openResult.Error > 0)
                    {
                        Console.WriteLine("ERROR : " + openResult.Error + ", " + openResult.ExtendedStatus);
                        Console.WriteLine("Enter to Exit");
                        Console.ReadLine();
                        return;
                    }

                    using (var udpClient = CreateImplictMessagingClient(openResult))
                    {
                        StartReceive(openResult, codec, udpClient);
                        StartSend(openResult, codec, udpClient, remoteAddress);
                        Console.WriteLine();
                        Console.WriteLine("Enter to ForwardClose");
                        Console.ReadLine();
                    }

                    codec.ForwardClose(stream, new ForwardCloseOptions(openResult));
                }
                finally
                {
                    codec.UnRegisterSession(stream);
                }

            }

        }

        public static void Identify(Stream stream, ENIPCodec codec)
        {
            var identifyObject = codec.CreateIdentifyObject(stream);
            Console.WriteLine("===== Start of Identify =====");
            Console.WriteLine("VenderID: " + identifyObject.VenderID);
            Console.WriteLine("DeviceType: " + identifyObject.DeviceType);
            Console.WriteLine("ProductCode: " + identifyObject.ProductCode);
            Console.WriteLine("Revision: " + identifyObject.Revision);
            Console.WriteLine("Status: " + identifyObject.Status);
            Console.WriteLine("SerialNumber: " + identifyObject.SerialNumber);
            Console.WriteLine("ProductName: " + identifyObject.ProductName);
            Console.WriteLine("===== End of Identify =====");
        }

        public static ForwardOpenResult ForwardOpen(Stream stream, ENIPCodec codec, IPAddress localAddess)
        {
            var openOptions = new ForwardOpenOptions();
            openOptions.LocalAddress = localAddess;
            openOptions.OriginatorUDPPort = 2222; // Support alternate port

            openOptions.O_T_Assembly.Length = 128;
            openOptions.O_T_Assembly.RealTimeFormat = RealTimeFormat.Header32Bit;
            openOptions.O_T_Assembly.ConnectionType = ConnectionType.PointToPoint;

            openOptions.T_O_Assembly.Length = 128;
            openOptions.T_O_Assembly.RealTimeFormat = RealTimeFormat.Modeless;
            openOptions.T_O_Assembly.ConnectionType = ConnectionType.PointToPoint;

            return codec.ForwardOpen(stream, openOptions);
        }

        public static UdpClient CreateImplictMessagingClient(ForwardOpenResult openResult)
        {
            var options = openResult.Options;
            var port = openResult.Options.OriginatorUDPPort;

            var receiver = new UdpClient();
            receiver.Client.Bind(new IPEndPoint(options.LocalAddress, port));

            if (openResult.Options.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                Console.WriteLine("JoinMulticastGroup : " + openResult.T_O_Address.Address + ", " + port);
                receiver.JoinMulticastGroup(openResult.T_O_Address.Address, options.LocalAddress);
            }

            return receiver;
        }

        public static void StartReceive(ForwardOpenResult openResult, ENIPCodec codec, UdpClient receiver)
        {
            var options = openResult.Options;
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        var bytes = receiver.Receive(ref remoteEP);

                        using (var ms = new MemoryStream(bytes))
                        {
                            var processor = new ENIPProcessor(ms);
                            var items = new CommandItems();
                            items.Read(processor, false);

                            var data = new byte[options.T_O_Assembly.Length];
                            codec.HandleImplicitTransmission(options.T_O_Assembly.RealTimeFormat, items, data);

                            Console.WriteLine(BitConverter.ToString(data));
                        }

                    }

                }
                catch
                {

                }

            }).Start();
        }

        public static void StartSend(ForwardOpenResult openResult, ENIPCodec codec, UdpClient sender, IPAddress remoteAddress)
        {
            var options = openResult.Options;
            var sendAddress = GetTargetEndPoint(openResult, remoteAddress);
            new Thread(() =>
            {
                try
                {
                    for (var i = 0u; ; i++)
                    {
                        var item = new CommandItemSequencedAddress();
                        item.SequenceCount = i;
                        item.ConnectionID = openResult.O_T_ConnectionID;

                        var data = new byte[options.O_T_Assembly.Length];

                        using (var ms = new MemoryStream())
                        {
                            var processor = new ENIPProcessor(ms);

                            for (var j = 0; j < options.O_T_Assembly.Length / 4; j++)
                            {
                                processor.WriteUInt(i);
                            }

                            data = ms.ToArray();
                        }

                        var items = codec.CreateImplicitTransmission(options.O_T_Assembly.RealTimeFormat, item, data);

                        using (var ms = new MemoryStream())
                        {
                            var processor = new ENIPProcessor(ms);
                            items.Write(processor);
                            sender.Send(ms.ToArray(), (int)ms.Length, sendAddress);
                            Thread.Sleep((int)(options.O_T_Assembly.RequestPacketRate / 1000U));
                        }

                    }

                }
                catch
                {

                }

            }).Start();
        }

        public static IPEndPoint GetTargetEndPoint(ForwardOpenResult openResult, IPAddress remoteEndPoint)
        {
            var options = openResult.Options;
            var endPoint = new IPEndPoint(remoteEndPoint, options.OriginatorUDPPort);

            if (openResult.O_T_Address != null)
            {
                endPoint.Address = openResult.O_T_Address.Address;
            }

            if (options.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                endPoint.Port = openResult.T_O_Address.Port;
            }

            return endPoint;
        }

    }

}
