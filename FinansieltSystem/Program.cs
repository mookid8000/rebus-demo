using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinansieltSystem.Messages;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;

namespace FinansieltSystem
{
    class Program
    {
        static readonly string SubscriptionsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\finansieltSystem.subscriptions.xml");

        static void Main()
        {
            using (var adapter = new BuiltinContainerAdapter())
            {
                Configure.With(adapter)
                    .Logging(l => l.None())
                    .Transport(t => t.UseMsmqAndGetInputQueueNameFromAppConfig())
                    .Subscriptions(s => s.StoreInXmlFile(SubscriptionsFilePath))
                    .CreateBus()
                    .Start();

                while (true)
                {
                    var købBogført = GenererKøbBogført();

                    Console.WriteLine("Bogfører køb for {0}", string.Join(", ", købBogført.Debitorer.Select(d => d.Cpr)));
                    adapter.Bus.Publish(købBogført);
                }
            }
        }

        static readonly Random Random = new Random();

        static KøbBogført GenererKøbBogført()
        {
            var købBogført = new KøbBogført();

            Console.WriteLine("Indast CPR-numre for debitorer");
            while(true)
            {
                Console.Write("> ");
                var cpr = Console.ReadLine();
                
                if (string.IsNullOrEmpty(cpr)) break;
                
                købBogført.Debitorer.Add(new Debitor {Cpr = cpr});
            }

            return købBogført;
        }
    }
}
