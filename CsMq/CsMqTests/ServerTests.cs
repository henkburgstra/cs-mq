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
                ""keepalive"": true,
                ""payload"": {}
            }";
            Message message = Server.MessageFromJson(testMsg);
            Assert.AreEqual(message.Sender, "sender");
            Assert.AreEqual(message.Function, "function");
            Assert.AreEqual(message.Relay, false);
            Assert.AreEqual(message.KeepAlive, true);
        }
    }
}