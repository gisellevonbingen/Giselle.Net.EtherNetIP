using System;
using System.Collections.Generic;
using System.Text;

namespace Giselle.Net.EtherNetIP.CIP
{
    public static class CommandItemRegister
    {
        public static readonly object SyncRoot = new object();
        public const int NotFoundId = -1;

        private static readonly Dictionary<ushort, CommandItemRegistration> IdMap = new Dictionary<ushort, CommandItemRegistration>();
        private static readonly Dictionary<Type, CommandItemRegistration> TypeMap = new Dictionary<Type, CommandItemRegistration>();

        static CommandItemRegister()
        {
            Register(0x0000, typeof(CommandItemNull));
            Register(0x00B1, typeof(CommandItemConnectedData));
            Register(0x00B2, typeof(CommandItemUnconnectedDataRequest), typeof(CommandItemUnconnectedDataResponse));
            Register(0x8000, typeof(CommandItemEndPoint_O_T));
            Register(0x8001, typeof(CommandItemEndPoint_T_O));
            Register(0x8002, typeof(CommandItemSequencedAddress));
        }

        public static void RequireNotRegister(ushort id, Type type)
        {
            string message = null;

            lock (SyncRoot)
            {
                if (IdMap.ContainsKey(id) == true)
                {
                    message = $"id({id}) has alreay registered";
                }

                if (TypeMap.ContainsKey(type) == true)
                {
                    message = $"type({type}) has alreay registered";
                }

            }

            if (message != null)
            {
                throw new ArgumentException(message);
            }

        }

        public static void Register(ushort id, Type commonType)
        {
            Register(id, commonType, commonType);
        }

        public static void Register(ushort id, Type requestType, Type responseType)
        {
            lock (SyncRoot)
            {
                RequireNotRegister(id, requestType);
                RequireNotRegister(id, responseType);

                var registration = new CommandItemRegistration(id, requestType, responseType);
                IdMap[id] = registration;
                TypeMap[requestType] = registration;
                TypeMap[responseType] = registration;
            }

        }

        public static CommandItemRegistration FromId(ushort id)
        {
            lock (SyncRoot)
            {
                return IdMap.TryGetValue(id, out var registration) ? registration : null;
            }

        }

        public static CommandItemRegistration FromType(Type type)
        {
            lock (SyncRoot)
            {
                return TypeMap.TryGetValue(type, out var registration) ? registration : null;
            }

        }

    }

}
