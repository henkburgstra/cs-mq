// Install-Package Newtonsoft.Json
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsMq
{
    [DataContract]
    public class Message
    {
        public const string MSG_BEGIN = "__BOF__";
        public const string MSG_END = "__EOF__";

        [DataMember(Name = "sender", IsRequired = true)]
        public string Sender;

        [DataMember(Name = "function", IsRequired = true)]
        public string Function;

        [DataMember(Name = "relay", IsRequired = true)]
        public bool Relay;

        [DataMember(Name = "keep_alive", IsRequired = true)]
        public bool KeepAlive;

        [DataMember(Name = "payload")]
        public JObject Payload { get; set; }

        public string Envelope(string data)
        {
            return String.Format("{0}{1}{2}", MSG_BEGIN, data, MSG_END);
        }

        public static Message FromJson(string json)
        {
            Trace.WriteLine(json);
            Message message = JsonConvert.DeserializeObject<Message>(json);
            return message;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
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

        public bool Send(Message message)
        {
            if (!Connection.Connected)
            {
                return false;
            }

            try
            {
                NetworkStream stream = Connection.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(message.Envelope(message.ToJson()));
                writer.Flush();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

    }

    public class Server
    {
        private TcpListener listener;
        private Regex reMsg = new Regex(string.Format("(.*)({0})(.*)({1})(.*)", Message.MSG_BEGIN, Message.MSG_END), RegexOptions.Singleline);

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
            string addr = "127.0.0.1";
            IPAddress ipAddr = IPAddress.Parse(addr);
            this.listener = new TcpListener(ipAddr, this.Port);
            try
            {
                this.listener.Start();
                Trace.WriteLine(String.Format("MessageQueue started at {0}:{1} ", addr, Port));
                while (this.KeepServing)
                {
                    TcpClient tcpClient = await this.listener.AcceptTcpClientAsync();
                    Task t = HandleClientAsync(tcpClient);
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

        private void AddClient(string id, TcpClient tcpclient)
        {
            var client = new Client(id, tcpclient);
            this.Clients.Add(id, client);
        }

        private void PurgeClients(List<string> clients)
        {
            foreach (var key in clients)
            {
                if (Clients.ContainsKey(key))
                {
                    Clients.Remove(key);
                }
            }
        }

        private void RelayMessage(Message message)
        {
            List<string> deadClients = new List<string>();

            foreach (var item in Clients)
            {
                if (item.Key == message.Sender)
                {
                    continue;
                }
                Client client = item.Value;
                var sent = client.Send(message);
                if (!sent)
                {
                    deadClients.Add(item.Key);
                }
            }

            if (deadClients.Count > 0)
            {
                PurgeClients(deadClients);
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
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
                        Message message = Message.FromJson(match.Groups[3].Value);
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
