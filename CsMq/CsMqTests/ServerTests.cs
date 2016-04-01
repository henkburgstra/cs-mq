using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsMq.Tests
{
    [TestClass()]
    public class ServerTests
    {
        [TestMethod()]
        public void MessageFromJsonTest()
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
            Message message = Server.MessageFromJson(testMsg);
            Assert.AreEqual(message.Sender, "sender");
            Assert.AreEqual(message.Function, "function");
            Assert.AreEqual(message.Relay, false);
            Assert.AreEqual(message.KeepAlive, true);
        }
    }
}