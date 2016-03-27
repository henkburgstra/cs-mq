using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsMq
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(3300);
            server.Start();
        }
    }
}
