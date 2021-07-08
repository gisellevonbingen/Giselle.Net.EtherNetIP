# Giselle.Net.EtherNetIP

- CIP EtherNet/IP
- Get/Set assembly data
- Implicit messaging
- Practice repository
- Capture/Analyze with WireShark

# Dependencies
- [Giselle.Commons](https://github.com/gisellevonbingen/Giselle.Commons)

# References
- https://github.com/boundary/wireshark/blob/master/epan/dissectors/packet-enip.c
- https://www.hilscher.com/fileadmin/cms_upload/ja/Resources/pdf/EtherNetIP_Adapter_V3_Protocol_API_04_EN.pdf

# Example

Giselle.Net.EtherNetIP.Test\Program.cs

```CSharp
using (var tcpClient = new TcpClient())
{
	tcpClient.Connect("192.168.1.51", 0xAF12);

	var stream = tcpClient.GetStream();
	var localAddress = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address;
	var codec = new ENIPCodec();

	try
	{
		codec.RegisterSession(stream);

		var identifyObject = codec.CreateIdentifyObject(stream);
		Console.WriteLine(identifyObject.VenderID);
		Console.WriteLine(identifyObject.DeviceType);
		Console.WriteLine(identifyObject.ProductCode);
		Console.WriteLine(identifyObject.Revision);
		Console.WriteLine(identifyObject.Status);
		Console.WriteLine(identifyObject.SerialNumber);
		Console.WriteLine(identifyObject.ProductName);

		Console.ReadLine();

		var openOptions = new ForwardOpenOptions();
		openOptions.Hostname = localAddress;

		openOptions.O_T_Assembly.Length = 128;
		openOptions.O_T_Assembly.RealTimeFormat = RealTimeFormat.Header32Bit;
		openOptions.O_T_Assembly.ConnectionType = ConnectionType.PointToPoint;

		openOptions.T_O_Assembly.Length = 128;
		openOptions.T_O_Assembly.RealTimeFormat = RealTimeFormat.Modeless;
		openOptions.T_O_Assembly.ConnectionType = ConnectionType.Multicast;

		var openResult = codec.ForwardOpen(stream, openOptions);

		if (openResult.Error > 0)
		{
			Console.WriteLine("ERROR : " + openResult.Error + ", " + openResult.ExtendedStatus);
			return;
		}

		var receiverEndPoint = new IPEndPoint(IPAddress.Any, openOptions.OriginatorUDPPort);

		using (var receiver = new UdpClient())
		{
			receiver.Client.Bind(receiverEndPoint);

			var senderEndPoint = new IPEndPoint(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address, openOptions.OriginatorUDPPort);

			if (openResult.O_T_Address != null)
			{
				senderEndPoint.Address = openResult.O_T_Address.Address;
			}

			if (openOptions.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
			{
				var multicastAddress = openResult.T_O_Address.Address;
				Console.WriteLine("Multicast : " + multicastAddress + ", " + openOptions.OriginatorUDPPort);

				receiver.JoinMulticastGroup(multicastAddress, localAddress);
				senderEndPoint.Port = openResult.T_O_Address.Port;
			}

			new Thread(() =>
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

						var data = new byte[openOptions.T_O_Assembly.Length];
						codec.HandleImplicitTransmission(openOptions.T_O_Assembly.RealTimeFormat, items, data);

						Console.WriteLine(BitConverter.ToString(data));
					}

				}

			}).Start();

			using (var sender = new UdpClient())
			{
				sender.Client.Bind(new IPEndPoint(localAddress, openOptions.OriginatorUDPPort));

				new Thread(() =>
				{
					for (var i = 0u; ; i++)
					{
						var item = new CommandItemSequencedAddress();
						item.SequenceCount = i;
						item.ConnectionID = openResult.O_T_ConnectionID;

						var data = new byte[openOptions.O_T_Assembly.Length];

						using (var ms = new MemoryStream())
						{
							var processor = new ENIPProcessor(ms);

							for (var j = 0; j < openOptions.O_T_Assembly.Length / 4; j++)
							{
								processor.WriteUInt(i);
							}

							data = ms.ToArray();
						}

						var items = codec.CreateImplicitTransmission(openOptions.O_T_Assembly.RealTimeFormat, item, data);

						using (var ms = new MemoryStream())
						{
							var processor = new ENIPProcessor(ms);
							items.Write(processor);
							sender.Send(ms.ToArray(), (int)ms.Length, senderEndPoint);
							Thread.Sleep((int)(openOptions.O_T_Assembly.RequestPacketRate / 1000U));
						}

					}

				}).Start();

				Console.ReadLine();

				codec.ForwardClose(stream, new ForwardCloseOptions(openResult));
			}

		}

	}
	finally
	{
		codec.UnRegisterSession(stream);
	}

}
```