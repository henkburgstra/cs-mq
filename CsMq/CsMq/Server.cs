using System;
using System.Collections.Generic;
using System.Linq;
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
            return true;
        }

    }
}
