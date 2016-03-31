using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsMq
{
    [DataContract]
    public class Message : IExtensibleDataObject
    {
        [DataMember(Name = "sender", IsRequired = true)]
        public string Sender;

        [DataMember(Name = "function", IsRequired = true)]
        public string Function;

        [DataMember(Name = "relay", IsRequired = true)]
        public bool Relay;

        [DataMember(Name = "keep_alive", IsRequired = true)]
        public bool KeepAlive;

        public ExtensionDataObject ExtensionData { get; set; }

    }

    public class Client
    {
        public Client(string id, TcpClient connection)
        {
            this.Id = id;
            this.Connection = connection;
        }

        public TcpClient Connection
        {
            get; set;
        }
        public string Id
        {
            get; set;
        }
        public bool Alive
        {
            get; set;
        }
        public static string MessageToJson(Message message)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Message));
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, message);
            StreamReader reader = new StreamReader(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return reader.ReadToEnd();
        }

        public void Send(Message message)
        {
            NetworkStream stream = Connection.GetStream();
            StreamWriter writer = new StreamWriter(stream);
 //           writer.Write(message.Payload);
            writer.Flush();
        }

    }

    public class Server
    {
        public const string MSG_BEGIN = "__BOF__";
        public const string MSG_END = "__EOF__";
        public const string KEEP_ALIVE = "KEEP_ALIVE";

        private TcpListener listener;
        private Regex reMsg = new Regex(string.Format("(.*)({0})(.*)({1})(.*)", MSG_BEGIN, MSG_END), RegexOptions.Singleline);

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

        public static Message MessageFromJson(string json)
        {
            Console.WriteLine(json);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Message));
            MemoryStream ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(json));
            Message message = (Message)js.ReadObject(ms);
            return message;
        }

        private void AddClient(string id, TcpClient tcpclient)
        {
            var client = new Client(id, tcpclient);
            this.Clients.Add(id, client);
        }

        private void RelayMessage(Message message)
        {
            foreach (var item in Clients)
            {
                if (item.Key == message.Sender)
                {
                    continue;
                }
                Client client = item.Value;
                client.Send(message);
            }
        }

        private async Task HandleClient(TcpClient tcpClient)
        {
            char[] buf = new char[2048];
            string data = "";
            bool closeConnection = true;

            try
            {
                NetworkStream stream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(stream);
                
                while (this.KeepServing)
                {
                    await reader.ReadAsync(buf, 0, 2048);
                    data += new string(buf);
                    Match match = reMsg.Match(data);
                    if (match.Success)
                    {
                        Message message = MessageFromJson(match.Groups[3].Value);
                        if (message.Relay == true)
                        {
                            RelayMessage(message);
                        }
                        if (message.KeepAlive)
                        {
                            closeConnection = false;
                            AddClient(message.Sender, tcpClient);
                        }
                        break;
                    }
                }
                if (closeConnection)
                {
                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
