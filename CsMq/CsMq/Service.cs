using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsMq
{
    class Service : ServiceBase
    {
        private Server server;

        protected override void OnStart(string[] args)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            int port = Properties.Settings.Default.messagequeue_port;
            server = new Server(port);
            server.Start();
            while (server.KeepServing)
            {
                Thread.Sleep(100);
            }
        }

        protected override void OnStop()
        {
            server.KeepServing = false;
        }
    }
}
