using System;
using FinansieltSystem.Messages;
using Kreditstatus.Messages;
using Rebus;
using System.Linq;

namespace Kreditstatus.Handlers
{
    class Igangs�tIndhentelseAfKreditstatus : IHandleMessages<K�bBogf�rt>
    {
        readonly IBus bus;

        public Igangs�tIndhentelseAfKreditstatus(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(K�bBogf�rt message)
        {
            Console.WriteLine("Modtog K�bBogf�rt med {0} debitorer: {1}",
                              message.Debitorer.Count, string.Join(", ", message.Debitorer.Select(d => d.Cpr)));

            foreach(var debitor in message.Debitorer)
            {
                bus.SendLocal(new S�rgForAtViHarOpdateredeOplysninger
                    {
                        Debitor = debitor
                    });
            }
        }
    }
}