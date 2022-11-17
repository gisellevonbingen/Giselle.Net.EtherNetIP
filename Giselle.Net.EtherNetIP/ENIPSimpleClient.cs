using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giselle.Net.EtherNetIP
{
    public class ENIPSimpleClient : IDisposable
    {
        public const int TcpPort = 0xAF12;

        private readonly ENIPCodec Codec;
        private readonly TcpClient TcpClient;

        private Stream TcpStream = null;
        private UdpClient UdpClient = null;
        private Thread ImplicitReceiveThread = null;
        private Thread ImplicitSendThread = null;

        public bool Connected { get; private set; } = false;
        /// <summary>
        /// Nullable
        /// </summary>
        public ForwardOpenResult LastForwardOpenResult { get; private set; } = null;

        public event EventHandler<Exception> ImplicitMessagingException;
        /// <summary>
        /// Fetch bytes from received
        /// </summary>
        public event EventHandler<byte[]> ImplicitMessageReceived;
        /// <summary>
        /// Fill bytes to sending
        /// </summary>
        public event EventHandler<byte[]> ImplicitMessageSending;

        public ENIPSimpleClient()
        {
            this.Codec = new ENIPCodec();
            this.TcpClient = new TcpClient();
        }

        public IdentifyObject CreateIdentifyObject()
        {
            this.EnsureConnected();
            return this.Codec.CreateIdentifyObject(this.TcpStream);
        }

        public void EnsureConnected()
        {
            if (this.Connected == false)
            {
                throw new IOException("Not connected");
            }

        }

        public void Connect(IPAddress hostname, int port = TcpPort) => this.Connect(new IPEndPoint(hostname, port));

        public void Connect(IPEndPoint hostname)
        {
            try
            {
                this.TcpClient.Connect(hostname);
                this.TcpStream = this.TcpClient.GetStream();
                this.Codec.RegisterSession(this.TcpStream);
                this.Connected = true;
            }
            catch (Exception)
            {
                this.Close();
                throw;
            }

        }

        public void Close()
        {
            this.ForwardClose();

            this.Connected = false;
            this.Codec.UnRegisterSession(this.TcpStream);
            this.TcpClient.DisposeQuietly();
            this.TcpStream.DisposeQuietly();
        }

        public DataProcessor GetAttribute(AttributePath path)
        {
            this.EnsureConnected();
            return this.Codec.GetAttribute(this.TcpStream, path);
        }

        public void SetAttribute(AttributePath path, byte[] bytes)
        {
            this.EnsureConnected();
            this.Codec.SetAttribute(this.TcpStream, path, bytes);
        }

        public void SetAttribute(AttributePath path, Action<DataProcessor> bytesMaker)
        {
            this.EnsureConnected();

            using (var ms = new MemoryStream())
            {
                bytesMaker(ENIPCodec.CreateDataProcessor(ms));
                this.Codec.SetAttribute(this.TcpStream, path, ms.ToArray());
            }

        }

        public Random Random { get => this.Codec.Random; set => this.Codec.Random = value; }

        public uint SessionId => this.Codec.SessionID;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">Set LocalAddress to connecting LocalEndPoint</param>
        /// <returns></returns>
        public ForwardOpenResult ForwardOpen(ForwardOpenOptions options)
        {
            this.EnsureConnected();
            this.ForwardClose();

            options.LocalAddress = ((IPEndPoint)this.TcpClient.Client.LocalEndPoint).Address;
            var result = this.Codec.ForwardOpen(this.TcpStream, options);
            this.LastForwardOpenResult = result;

            this.UdpClient = this.CreateImplictMessagingClient(result);
            this.ImplicitReceiveThread = new Thread(this.RunImplicitReceive);
            this.ImplicitReceiveThread.Start();
            this.ImplicitSendThread = new Thread(this.RunImplicitSend);
            this.ImplicitSendThread.Start();

            return result;
        }

        private UdpClient CreateImplictMessagingClient(ForwardOpenResult openResult)
        {
            var options = openResult.Options;
            var port = 2222; // Receiving port is fixed 2222, in my test

            var udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(options.LocalAddress, port));

            if (openResult.Options.T_O_Assembly.ConnectionType == ConnectionType.Multicast)
            {
                udpClient.JoinMulticastGroup(openResult.T_O_Address.Address, options.LocalAddress);
            }

            return udpClient;
        }

        private void RunImplicitReceive()
        {
            var udpClient = this.UdpClient;
            var options = this.LastForwardOpenResult.Options;

            try
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    var bytes = udpClient.Receive(ref remoteEP);

                    using (var ms = new MemoryStream(bytes))
                    {
                        var items = new CommandItems();
                        items.Read(ENIPCodec.CreateDataProcessor(ms), false);

                        var data = new byte[options.T_O_Assembly.Length];
                        this.Codec.HandleImplicitTransmission(options.T_O_Assembly.RealTimeFormat, items, data);
                        this.OnImplicitMessageReceived(data);
                    }

                }

            }
            catch (ThreadInterruptedException)
            {

            }
            catch (Exception e)
            {
                this.OnImplicitMessagingException(e);
            }

        }

        private void RunImplicitSend()
        {
            var udpClient = this.UdpClient;
            var result = this.LastForwardOpenResult;
            var options = result.Options;
            var targetEndPoint = new IPEndPoint(((IPEndPoint)this.TcpClient.Client.RemoteEndPoint).Address, options.T_O_UDPPort);

            try
            {
                for (var i = 0u; ; i++)
                {
                    var item = new CommandItemSequencedAddress();
                    item.SequenceCount = i;
                    item.ConnectionID = result.O_T_ConnectionID;

                    var data = new byte[options.O_T_Assembly.Length];
                    this.OnImplicitMessageSending(data);

                    var items = this.Codec.CreateImplicitTransmission(options.O_T_Assembly.RealTimeFormat, item, data);

                    using (var ms = new MemoryStream())
                    {
                        items.Write(ENIPCodec.CreateDataProcessor(ms));
                        udpClient.Send(ms.ToArray(), (int)ms.Length, targetEndPoint);
                    }

                    Thread.Sleep((int)(options.O_T_Assembly.RequestPacketRate / 1000U));
                }

            }
            catch (ThreadInterruptedException)
            {

            }
            catch (Exception e)
            {
                this.OnImplicitMessagingException(e);
            }

        }

        public void ForwardClose()
        {
            if (this.LastForwardOpenResult == null)
            {
                return;
            }

            this.UdpClient.DisposeQuietly();
            this.ImplicitReceiveThread.InterruptQuietly();
            this.ImplicitSendThread.InterruptQuietly();
            this.EnsureConnected();

            this.Codec.ForwardClose(this.TcpStream, new ForwardCloseOptions(this.LastForwardOpenResult));
            this.LastForwardOpenResult = null;
        }

        protected virtual void OnImplicitMessagingException(Exception e)
        {
            this.ImplicitMessagingException?.Invoke(this, e);
        }

        protected virtual void OnImplicitMessageReceived(byte[] bytes)
        {
            this.ImplicitMessageReceived?.Invoke(this, bytes);
        }

        protected virtual void OnImplicitMessageSending(byte[] bytes)
        {
            this.ImplicitMessageSending?.Invoke(this, bytes);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Close();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        ~ENIPSimpleClient()
        {
            this.Dispose(false);
        }

    }

}
