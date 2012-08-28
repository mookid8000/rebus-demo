using System;
using FinansieltSystem.Messages;
using Kreditstatus.Messages;
using Rebus;
using System.Linq;

namespace Kreditstatus.Handlers
{
    class IgangsætIndhentelseAfKreditstatus : IHandleMessages<KøbBogført>
    {
        readonly IBus bus;

        public IgangsætIndhentelseAfKreditstatus(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(KøbBogført message)
        {
            Console.WriteLine("Modtog KøbBogført med {0} debitorer: {1}",
                              message.Debitorer.Count, string.Join(", ", message.Debitorer.Select(d => d.Cpr)));

            foreach(var debitor in message.Debitorer)
            {
                bus.SendLocal(new SørgForAtViHarOpdateredeOplysninger
                    {
                        Debitor = debitor
                    });
            }
        }
    }
}