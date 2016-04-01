using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsMq
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            int port = Properties.Settings.Default.messagequeue_port;
            var server = new Server(port);
            server.Start();
            while (server.KeepServing)
            {
                Thread.Sleep(100);
            }
        }
    }
}
