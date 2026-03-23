using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class ENIPSimpleClient : IDisposable
    {
        public const int TcpPort = 0xAF12;

        private readonly ENIPCodec Codec;
        private readonly TcpClient TcpClient;

        private Stream TcpStream = null;
        private DataProcessor TcpProcessor = null;
        private UdpClient UdpClient = null;
        private Thread ImplicitReceiveThread = null;
        private Thread ImplicitSendThread = null;

        public bool Connected { get; private set; } = false;
        /// <summary>
        /// Nullable
        /// </summary>
        public ForwardOpenOptions LastForwardOpenOptions { get; private set; } = null;
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

        public IdentifyAttributes GetIdentifyAttributes()
        {
            return new IdentifyAttributes(this.GetAttribute);
        }

        public ClassAttributes GetClassAttributes(uint classId)
        {
            return new ClassAttributes(this.GetAttribute, classId);
        }

        public void Connect(IPAddress hostname, int port = TcpPort) => this.Connect(new IPEndPoint(hostname, port));

        public void Connect(IPEndPoint hostname)
        {
            try
            {
                this.TcpClient.Connect(hostname);
                this.TcpStream = this.TcpClient.GetStream();
                this.TcpProcessor = CIPCodec.CreateDataProcessor(this.TcpStream);

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
            this.UnRegisterSession();
            this.Connected = false;

            this.TcpClient.DisposeQuietly();
            this.TcpStream.DisposeQuietly();
        }

        private void WriteEncapsulation(Encapsulation request)
        {
            if (this.Connected == false)
            {
                throw new IOException("Not connected");
            }

            using (var ms = new MemoryStream())
            {
                request.Write(ENIPCodec.CreateDataProcessor(ms));
                var bytes = ms.ToArray();
                this.TcpStream.Write(bytes, 0, bytes.Length);
            }

        }

        private Encapsulation ReadEncapsulation()
        {
            return new Encapsulation(this.TcpProcessor);
        }

        public Encapsulation ExchangeEncapsulation(Encapsulation request)
        {
            this.WriteEncapsulation(request);
            return this.ReadEncapsulation();
        }

        public CommandItems ExchangeSendRRData(params CommandItem[] requests)
        {
            return this.ExchangeSendRRData(this.Codec.CreateSendRRData(requests));
        }

        public CommandItems ExchangeSendRRData(IEnumerable<CommandItem> requests)
        {
            return this.ExchangeSendRRData(this.Codec.CreateSendRRData(requests));
        }

        public CommandItems ExchangeSendRRData(SendRRData request)
        {
            var requestEncapsulation = this.Codec.CreateEncapsulation(request);
            var responseEncapsulation = this.ExchangeEncapsulation(requestEncapsulation);

            return this.Codec.HandleEncapsulation(responseEncapsulation, false).Items;
        }

        public RES ExchangeSendRRData<RES>(Func<CommandItems, RES> responseFunc, params CommandItem[] requests)
        {
            return responseFunc(this.ExchangeSendRRData(requests));
        }

        public RES ExchangeSendRRData<RES>(Func<CommandItems, RES> responseFunc, IEnumerable<CommandItem> requests)
        {
            return responseFunc(this.ExchangeSendRRData(requests));
        }

        public uint RegisterSession()
        {
            var request = this.Codec.CreateRegisterSession();
            var response = this.ExchangeEncapsulation(request);
            return this.Codec.HandleRegisterSession(response);
        }

        public void UnRegisterSession()
        {
            var request = this.Codec.CreateUnRegisterSession();
            this.WriteEncapsulation(request);
            this.Codec.HandleUnRegisterSession();
        }

        public DataProcessor GetAssemblyData(uint instanceId) => this.GetAttribute(new AttributePath(KnownClassId.Assembly, instanceId, KnownAssembyAttributeId.Data));

        public ushort GetAssemblySize(uint instanceId) => this.GetAttribute(new AttributePath(KnownClassId.Assembly, instanceId, KnownAssembyAttributeId.Size)).ReadUShort();

        public void SetAssemblyData(uint instanceId, byte[] bytes) => this.SetAttribute(new AttributePath(KnownClassId.Assembly, instanceId, KnownAssembyAttributeId.Data), bytes);

        public void SetAssemblyData(uint instanceId, Action<DataProcessor> bytesMaker)
        {
            using (var ms = new MemoryStream())
            {
                bytesMaker(CIPCodec.CreateDataProcessor(ms));
                this.SetAssemblyData(instanceId, ms.ToArray());
            }

        }

        public DataProcessor GetAttribute(AttributePath path)
        {
            return this.ExchangeSendRRData(this.Codec.HandleGetAttribute, this.Codec.CreateGetAttribute(path));
        }

        public byte SetAttribute(AttributePath path, byte[] bytes)
        {
            return this.ExchangeSendRRData(this.Codec.HandleSetAttribute, this.Codec.CreateSetAttribute(path, bytes));
        }

        public byte SetAttribute(AttributePath path, Action<DataProcessor> bytesMaker)
        {
            using (var ms = new MemoryStream())
            {
                bytesMaker(CIPCodec.CreateDataProcessor(ms));
                return this.SetAttribute(path, ms.ToArray());
            }

        }

        public Random Random { get => this.Codec.Random; set => this.Codec.Random = value; }

        public uint SessionId => this.Codec.SessionId;

        public bool ImplicitRun { get => this.Codec.ImplicitRun; set => this.Codec.ImplicitRun = value; }
        public bool ImplicitCOO { get => this.Codec.ImplicitCOO; set => this.Codec.ImplicitCOO = value; }
        public byte ImplicitROO { get => this.Codec.ImplicitROO; set => this.Codec.ImplicitROO = value; }

        public ForwardOpenResult ForwardOpen(ForwardOpenOptions options)
        {
            this.ForwardClose();

            options = new ForwardOpenOptions(options);
            var localAddress = ((IPEndPoint)this.TcpClient.Client.LocalEndPoint).Address;

            var result = this.ExchangeSendRRData(this.Codec.HandleForwardOpen, this.Codec.CreateForwardOpen(options, localAddress));
            this.LastForwardOpenOptions = new ForwardOpenOptions(options);
            this.LastForwardOpenResult = new ForwardOpenResult(result);

            this.UdpClient = this.Codec.CreateImplictMessagingClient(options, result, localAddress);
            this.ImplicitReceiveThread = new Thread(() => this.RunImplicitReceive(options));
            this.ImplicitReceiveThread.Start();
            this.ImplicitSendThread = new Thread(() => this.RunImplicitSend(options, result.O_T_ConnectionId));
            this.ImplicitSendThread.Start();

            return this.LastForwardOpenResult;
        }

        private void RunImplicitReceive(ForwardOpenOptions options)
        {
            var udpClient = this.UdpClient;

            try
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    var bytes = udpClient.Receive(ref remoteEP);

                    using (var ms = new MemoryStream(bytes))
                    {
                        var items = new CommandItems(CIPCodec.CreateDataProcessor(ms), false);

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

        private void RunImplicitSend(ForwardOpenOptions options, uint otConnectionId)
        {
            var udpClient = this.UdpClient;
            var targetEndPoint = new IPEndPoint(((IPEndPoint)this.TcpClient.Client.RemoteEndPoint).Address, options.O_T_UDPPort);

            try
            {
                for (var i = 0U; ; i++)
                {
                    var item = new CommandItemSequencedAddress()
                    {
                        SequenceCount = i,
                        ConnectionId = otConnectionId,
                    };

                    var data = new byte[options.O_T_Assembly.Length];
                    this.OnImplicitMessageSending(data);

                    var items = this.Codec.CreateImplicitTransmission(options.O_T_Assembly.RealTimeFormat, item, data);

                    using (var ms = new MemoryStream())
                    {
                        items.Write(CIPCodec.CreateDataProcessor(ms));
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

            var options = new ForwardCloseOptions(this.LastForwardOpenOptions, this.LastForwardOpenResult);
            this.ExchangeSendRRData(this.Codec.HandleForwardClose, this.Codec.CreateForwardClose(options));

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
