using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CsMq
{
    [DataContract]
    public struct Message
    {
        [DataMember(Name = "sender", IsRequired = true)]
        public string Sender;

        [DataMember(Name = "function", IsRequired = true)]
        public string Function;

        [DataMember(Name = "relay", IsRequired = true)]
        public bool Relay;

        [DataMember(Name = "keepalive", IsRequired = true)]
        public bool KeepAlive;

        [DataMember(Name = "payload")]
        public string Payload;

    }

    public class Client
    {
        public string Id
        {
            get; set;
        }
        public bool Alive
        {
            get; set;
        }

    }

    public class Server
    {
        private TcpListener listener;

        public int Port
        {
            get; set;
        }

        public Dictionary<string, Client> Clients = new Dictionary<string, Client>();

        public bool KeepServing
        {
            get; set;
        }

        public Server(int port)
        {
            this.Port = port;
        }

        public bool Start()
        {
            bool started = true;
            IPAddress adr = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(adr, this.Port);
            try
            {
                this.listener.Start();
                this.KeepServing = true;
                Task.Run(() => HandleClient());
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                started = false;
            }
            finally
            {
                this.listener.Stop();
            }
            return started;
        }
        private async Task HandleClient()
        {
            while (KeepServing)
            {
                var client = await listener.AcceptTcpClientAsync();
            }
        }

    }
}
