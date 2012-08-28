using System;
using System.Threading;
using Kreditbandit.Messages;
using Rebus;

namespace Kreditbandit.Handlers
{
    public class Indhenter : IHandleMessages<IndhentKreditstatusFraDebitorregisteret>,
        IHandleMessages<IndhentKreditstatusFraRki>
    {
        readonly IBus bus;
        readonly Random random = new Random();

        public Indhenter(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(IndhentKreditstatusFraDebitorregisteret message)
        {
            Console.WriteLine("Indhenter kreditstatus fra Debitorregisteret for {0}", message.Cpr);

            VentLidt();

            if (ViLaderSomOmDerSketeEnFejl())
            {
                Console.WriteLine("Noget gik galt for {0}... vi hørte aldrig fra Debitorregisteret",
                                  message.Cpr);
                return;
            }

            bus.Reply(new Kreditstatussvar
                {
                    Cpr = message.Cpr,
                    ThumbsUp = random.Next(2) == 0,
                    HvorKomDetFra = "Det var så Debitorregisteret"
                });
        }

        public void Handle(IndhentKreditstatusFraRki message)
        {
            Console.WriteLine("Indhenter kreditstatus fra RKI for {0}", message.Cpr);

            VentLidt();

            if (ViLaderSomOmDerSketeEnFejl())
            {
                Console.WriteLine("Noget gik galt for {0}... vi hørte aldrig fra RKI",
                                  message.Cpr);
                return;
            }

            bus.Reply(new Kreditstatussvar
                {
                    Cpr = message.Cpr,
                    ThumbsUp = random.Next(2) == 0,
                    HvorKomDetFra = "Det var så RKI"
                });
        }

        void VentLidt()
        {
            Thread.Sleep(TimeSpan.FromSeconds(random.Next(2) + 2));
        }

        bool ViLaderSomOmDerSketeEnFejl()
        {
            return random.Next(3) == 0;
        }
    }
}