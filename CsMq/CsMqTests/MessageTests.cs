using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using CsMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsMq.Tests
{
    [TestClass()]
    public class MessageTests
    {
        string testMsg = @"
            {
                ""sender"": ""sender"",
                ""function"": ""function"",
                ""relay"": false,
                ""keep_alive"": true,
                ""payload"": {
                    ""begindatum"": ""2016-03-31"",
                    ""einddatum"": ""2016-03-13"",
                    ""periode"": ""dagelijks"",
                    ""timestamp"": ""2016-03-31 15:54:48"",
                    ""uuid"": ""220e32cf-f748-11e5-914c-6c626d98ae88""
                }
            }";
        [TestMethod()]
        public void ToJsonTest()
        {
            Message message = Message.FromJson(testMsg);
            string serializedMsg = message.ToJson();
            Message message2 = Message.FromJson(serializedMsg);
            string fragment = JsonConvert.SerializeObject(message2.Payload);

            Assert.AreEqual(message.Payload.GetValue("begindatum").ToString(),
                message2.Payload.GetValue("begindatum").ToString());
        }

        [TestMethod()]
        public void FromJsonTest()
        {
            Message message = Message.FromJson(testMsg);
            Assert.AreEqual(message.Sender, "sender");
            Assert.AreEqual(message.Function, "function");
            Assert.AreEqual(message.Relay, false);
            Assert.AreEqual(message.KeepAlive, true);
        }
    }
}