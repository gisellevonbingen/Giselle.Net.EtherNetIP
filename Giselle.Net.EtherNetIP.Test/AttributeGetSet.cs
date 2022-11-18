using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class AttributeGetSet
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


                Console.WriteLine("Get: ClassID.InstanceID.AttributeID");
                Console.WriteLine("Set: ClassID.InstanceID.AttributeID=Value1-Value2-...Value(n)");
                Console.WriteLine("Example");
                Console.WriteLine("Get ProductName: 1.0.7");
                Console.WriteLine("Set Aseembly   : 4.100.3=34-12");

                while (true)
                {
                    Console.WriteLine();
                    Console.Write("Enter Commandline: ");
                    var line = Console.ReadLine();

                    if (line.Equals(":exit") == true)
                    {
                        break;
                    }

                    var split = line.Split('=');
                    var segments = split[0].Split('.').Select(uint.Parse).ToArray();
                    var path = new AttributePath(segments[0], segments[1], segments[2]);

                    if (split.Length > 1)
                    {
                        var bytes = split[1].Split('-').Select(s => byte.Parse(s, NumberStyles.HexNumber)).ToArray();
                        var error = client.SetAttribute(path, bytes);
                        Console.WriteLine("Error: " + error);
                    }
                    else
                    {
                        var result = client.GetAttribute(path);
                        var bytes = result.ReadBytes(result.Remain);
                        Console.WriteLine($"{path}={BitConverter.ToString(bytes)}");
                    }

                }

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

    }

}
