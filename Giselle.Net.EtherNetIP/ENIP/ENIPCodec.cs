using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Giselle.Net.EtherNetIP.CIP;

namespace Giselle.Net.EtherNetIP.ENIP
{
    public class ENIPCodec
    {
        public static DataProcessor CreateDataProcessor(Stream baseStream) => CIPCodec.CreateDataProcessor(baseStream);

        public CIPCodec CIPCodec { get; set; }
        public uint SessionID { get; set; }
        public ushort SendRRDataTimeout { get; set; }

        public ENIPCodec()
        {
            this.CIPCodec = new CIPCodec();
            this.SessionID = 0;
            this.SendRRDataTimeout = 0;
        }

        public IdentifyAttributes GetIdentifyAttributes(Stream stream) => new IdentifyAttributes(this, stream);

        public ClassAttributes GetClassAttributes(Stream stream, uint classId) => new ClassAttributes(this, stream, classId);

        public void WriteEncapsulation(Stream stream, Encapsulation request)
        {
            using (var ms = new MemoryStream())
            {
                var processor = CreateDataProcessor(ms);
                processor.WriteEncapsulation(request);

                var bytes = ms.ToArray();
                stream.Write(bytes, 0, bytes.Length);
            }

        }

        public Encapsulation ReadEncapsulation(Stream stream)
        {
            return CreateDataProcessor(stream).ReadEncapsulation();
        }

        public Encapsulation ExchangeEncapsulation(Stream stream, Encapsulation request)
        {
            this.WriteEncapsulation(stream, request);
            return this.ReadEncapsulation(stream);
        }

        public Encapsulation CreateEncapsulation()
        {
            var encapsulation = new Encapsulation();
            encapsulation.SessionID = this.SessionID;

            return encapsulation;
        }

        public Encapsulation CreateEncapsulation(SendRRData data)
        {
            var encapsulation = this.CreateEncapsulation();
            encapsulation.Command = EncapsulationCommand.SendRRData;
            data.Write(encapsulation.DataProcessor);

            return encapsulation;
        }

        public SendRRData ReadCommandData(Encapsulation response, bool isRequest) => response.DataProcessor.ReadCommandData(isRequest);

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
                var response = this.ExchangeEncapsulation(stream, request);
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

        public SendRRData CreateSendRRData(params CommandItem[] items) => this.CreateSendRRData((IEnumerable<CommandItem>)items);

        public SendRRData CreateSendRRData(IEnumerable<CommandItem> items) => new SendRRData(items) { Timeout = this.SendRRDataTimeout };

        public CommandItems ExchangeSendRRData(Stream stream, params CommandItem[] requests)
        {
            var req = this.CreateSendRRData(requests);
            return this.ExchangeSendRRData(stream, req);
        }

        public CommandItems ExchangeSendRRData(Stream stream, SendRRData request)
        {
            var req = this.CreateEncapsulation(request);
            var res = this.ExchangeEncapsulation(stream, req);
            return this.ReadCommandData(res, false).Items;
        }

        private RES ExchangeSendRRData<RES>(Stream stream, Func<CommandItems, RES> responseFunc, params CommandItem[] requests)
        {
            var response = this.ExchangeSendRRData(stream, requests);
            return responseFunc(response);
        }

        public DataProcessor GetAttribute(Stream stream, AttributePath path) => this.ExchangeSendRRData(stream,
            this.CIPCodec.HandleGetAttribute,
            this.CIPCodec.CreateGetAttribute(path));


        public byte SetAttribute(Stream stream, AttributePath path, byte[] values) => this.ExchangeSendRRData(stream,
            this.CIPCodec.HandleSetAttribute,
            this.CIPCodec.CreateSetAttribute(path, values));

        public ForwardOpenResult ForwardOpen(Stream stream, ForwardOpenOptions options) => this.ExchangeSendRRData(stream,
            response => this.CIPCodec.HandleForwardOpen(response, options),
            this.CIPCodec.CreateForwardOpen(options));

        public ForwardCloseResult ForwardClose(Stream stream, ForwardCloseOptions options) => this.ExchangeSendRRData(stream,
            response => this.CIPCodec.HandleForwardClose(response, options),
            this.CIPCodec.CreateForwardClose(options));

    }

}
