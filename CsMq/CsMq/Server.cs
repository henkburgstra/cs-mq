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
        [DataMember]
        public String Sender;

        [DataMember]
        public String Function;

        [DataMember]
        public Boolean Relay;

        [DataMember]
        public Boolean KeepAlive;
    }

    class Server
    {
    }
}
