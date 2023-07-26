using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Giselle.Net.EtherNetIP.CIP;
using Giselle.Net.EtherNetIP.ENIP;

namespace Giselle.Net.EtherNetIP.Test
{
    public class ImplicitMessaging
    {
        public static void Main()
        {
            using (var client = new ENIPSimpleClient())
            {
                try
                {
                    Console.Write("Enter IPAdress: ");
                    var hostname = Console.ReadLine();
                    Console.WriteLine("Connecting");
                    client.Connect(IPAddress.Parse(hostname));
                    Console.WriteLine("Connected");
                }
                catch
                {
                    Console.WriteLine("ERROR : Can't connect");
                    Console.WriteLine("Enter to Exit");
                    Console.ReadLine();
                    return;
                }

                Identify(client);

                Console.WriteLine("Enter to ForwardOpen");
                Console.ReadLine();

                // Events for use Implicit Messaging
                client.ImplicitMessagingException += (sender, exception) => Console.WriteLine($"Exception: {exception}");
                client.ImplicitMessageReceived    += (sender, bytes)     => Console.WriteLine($"Received: {BitConverter.ToString(bytes)}");
                client.ImplicitMessageSending     += (sender, bytes)     => FillSendingBytes(client, bytes);

                var openResult = ForwardOpen(client);

                if (openResult.Error > 0)
                {
                    Console.WriteLine($"ERROR : {openResult.Error}, {openResult.ExtendedStatus}");
                    Console.WriteLine("Enter to Exit");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Enter to ForwardClose");
                Console.ReadLine();
                client.ForwardClose();
            }

        }

        public static void Identify(ENIPSimpleClient client)
        {
            var identifyAttributes = client.GetIdentifyAttributes();
            var identifyClassAttributes = identifyAttributes.ClassAttributes;

            Console.WriteLine("===== Start of Identify Class Attributes =====");
            Console.WriteLine($"Revision: {identifyClassAttributes.Revision}");
            Console.WriteLine($"InstanceMaxID: {identifyClassAttributes.InstanceMaxID}");
            Console.WriteLine($"InstanceCount: {identifyClassAttributes.InstanceCount}");
            Console.WriteLine($"ClassAttributesMaxID: {identifyClassAttributes.ClassAttributesMaxID}");
            Console.WriteLine($"InstanceAttributesMaxID: {identifyClassAttributes.InstanceAttributesMaxID}");
            Console.WriteLine("===== End of Identify Class Attributes =====");
            Console.WriteLine();

            Console.WriteLine("===== Start of Identify Attributes =====");
            Console.WriteLine($"VenderID: {identifyAttributes.VenderID}");
            Console.WriteLine($"DeviceType: {identifyAttributes.DeviceType}");
            Console.WriteLine($"ProductCode: {identifyAttributes.ProductCode}");
            Console.WriteLine($"Revision: {identifyAttributes.Revision}");
            Console.WriteLine($"Status: {identifyAttributes.Status}");
            Console.WriteLine($"SerialNumber: {identifyAttributes.SerialNumber}");
            Console.WriteLine($"ProductName: {identifyAttributes.ProductName}");
            Console.WriteLine("===== End of Identify Attributes =====");
            Console.WriteLine();
        }

        public static ForwardOpenResult ForwardOpen(ENIPSimpleClient client)
        {
            var openOptions = new ForwardOpenOptions();
            openOptions.T_O_UDPPort = 2222; // Support alternate port, Default is 2222

            // T : Target
            // O : Originator

            openOptions.O_T_Assembly.InstanceID = 101; // Your O->T assembly instance id
            openOptions.O_T_Assembly.Length = 64;
            openOptions.O_T_Assembly.RealTimeFormat = RealTimeFormat.Header32Bit;
            openOptions.O_T_Assembly.ConnectionType = ConnectionType.PointToPoint;
            openOptions.O_T_Assembly.RequestPacketRate = 50000; // 50,000 ns = 50 ms;

            openOptions.T_O_Assembly.InstanceID = 100; // Your T->O assembly instance id
            openOptions.T_O_Assembly.Length = 64;
            openOptions.T_O_Assembly.RealTimeFormat = RealTimeFormat.Modeless;
            openOptions.T_O_Assembly.ConnectionType = ConnectionType.PointToPoint;
            openOptions.T_O_Assembly.RequestPacketRate = 50000; // 50,000 ns = 50 ms;

            return client.ForwardOpen(openOptions);
        }

        private static int FillingCycle = 0;

        private static void FillSendingBytes(ENIPSimpleClient client, byte[] bytes)
        {
            var dwords = client.LastForwardOpenResult.Options.O_T_Assembly.Length / 4;
            var filling = BitConverter.GetBytes(++FillingCycle);

            for (var i = 0; i < dwords; i++)
            {
                Array.Copy(filling, 0, bytes, i * 4, 4);
            }

            Console.WriteLine($"Sending: {FillingCycle:X2}");
        }

    }

}
