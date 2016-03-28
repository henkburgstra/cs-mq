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
        public int Port
        {
            get; set;
        }

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
            IPAddress adr = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(adr, this.Port);
            try
            {
                listener.Start();
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                listener.Stop();
            }
            return true;
        }

    }
}
