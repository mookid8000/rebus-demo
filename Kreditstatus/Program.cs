using System;
using FinansieltSystem.Messages;
using Kreditstatus.Handlers;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;
using Rebus.MongoDb;

namespace Kreditstatus
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
                    .DetermineEndpoints(d => d.FromRebusConfigurationSection())
                    .Sagas(s => s.StoreInMongoDb("mongodb://localhost:27017/kreditstatus")
                                    .SetCollectionName<SørgForOpdateredeOplysningerSagaData>("sagaer"))
                    .CreateBus();

                adapter
                    .Register(() => new IgangsætIndhentelseAfKreditstatus(adapter.Bus))
                    .Register(() => new SørgForOpdateredeOplysningerSaga(adapter.Bus));

                startable.Start();

                adapter.Bus.Subscribe<KøbBogført>();

                Console.WriteLine("Tryk enter...");
                Console.ReadLine();
            }
        }
    }
}
