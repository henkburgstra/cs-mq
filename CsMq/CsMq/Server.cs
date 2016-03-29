using System;
using System.Collections.Generic;
using System.IO;
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
            this.KeepServing = true;
        }

        public async void Start()
        {
            IPAddress adr = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(adr, this.Port);
            try
            {
                this.listener.Start();
                while (this.KeepServing)
                {
                    TcpClient tcpClient = await this.listener.AcceptTcpClientAsync();
                    Task t = HandleClient(tcpClient);
                    await t;
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                this.listener.Stop();
            }
        }

        private async Task HandleClient(TcpClient tcpClient)
        {
            char[] buf = new char[2048];

            try
            {
                NetworkStream stream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                
                while (this.KeepServing)
                {
                    int count = await reader.ReadAsync(buf, 0, 2048);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
