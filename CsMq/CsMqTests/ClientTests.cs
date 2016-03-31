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
    public class ClientTests
    {
        [TestMethod()]
        public void MessageToJsonTest()
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
            string serializedMsg = Client.MessageToJson(message);
            Assert.AreEqual(testMsg, serializedMsg);
        }
    }
}