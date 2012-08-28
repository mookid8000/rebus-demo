using System;
using Kreditbandit.Handlers;
using Rebus.Configuration;
using Rebus.Transports.Msmq;
using Rebus.Logging;

namespace Kreditbandit
{
    class Program
    {
        static void Main()
        {
            using(var adapter = new BuiltinContainerAdapter())
            {
                var startable = Configure.With(adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UseMsmqAndGetInputQueueNameFromAppConfig())
                    .CreateBus();

                adapter.Register(() => new Indhenter(adapter.Bus));

                startable.Start();

                Console.WriteLine("Tryk enter...");
                Console.ReadLine();
            }
        }
    }
}
