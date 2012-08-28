using System;
using System.Collections.Generic;
using Kreditbandit.Messages;
using Kreditstatus.Messages;
using Rebus;
using System.Linq;

namespace Kreditstatus.Handlers
{
    public class SørgForOpdateredeOplysningerSaga : Saga<SørgForOpdateredeOplysningerSagaData>,
        IAmInitiatedBy<SørgForAtViHarOpdateredeOplysninger>,
        IHandleMessages<Kreditstatussvar>,
        IHandleMessages<VerificerAtOplysningerErBlevetOpdateret>
    {
        readonly IBus bus;

        public SørgForOpdateredeOplysningerSaga(IBus bus)
        {
            this.bus = bus;
        }

        public override void ConfigureHowToFindSaga()
        {
            Incoming<SørgForAtViHarOpdateredeOplysninger>(m => m.Debitor.Cpr).CorrelatesWith(d => d.Cpr);
            Incoming<Kreditstatussvar>(m => m.Cpr).CorrelatesWith(d => d.Cpr);
            Incoming<VerificerAtOplysningerErBlevetOpdateret>(m => m.Cpr).CorrelatesWith(d => d.Cpr);
        }

        public void Handle(SørgForAtViHarOpdateredeOplysninger message)
        {
            if (IsNew)
            {
                Data.Cpr = message.Debitor.Cpr;
            }

            if (ViHarEnFriskStatus())
            {
                Console.WriteLine("Status for {0} var frisk nok - tag en slapper...", Data.Cpr);
                return;
            }
            
            if (Data.ViAfventerStatussvar)
            {
                Console.WriteLine(
                    "Vi har ingen frisk status for {0}, men vi har bestilt en {1}... lad os bare vente lidt",
                    Data.Cpr, Data.TidspunktForBestillingAfStatus);
                return;
            }
            
            Console.WriteLine("Vi har ingen frisk status for {0}, så vi bestiller en ny...", Data.Cpr);

            Data.Gentagelser = 0;
            IndhentKreditstatusser();
        }

        public void Handle(Kreditstatussvar message)
        {
            Console.WriteLine("Der kom et svar for {0}: {1}", message.Cpr, message.HvorKomDetFra);

            Data.Statusser.Add(new Kreditstatus
                {
                    Hvorfra = message.HvorKomDetFra,
                    ThumbsUp = message.ThumbsUp,
                    Hvornår = DateTime.UtcNow,
                });

            Data.ViAfventerStatussvar = false;

            Console.WriteLine(message.ThumbsUp ? Tekster.GodTekst : Tekster.DårligTekst);
        }

        public void Handle(VerificerAtOplysningerErBlevetOpdateret message)
        {
            if (ViHarEnFriskStatus())
            {
                Console.WriteLine("Vi har en frisk status for {0}, så vi behøver ikke gøre mere....",
                                  Data.Cpr);
                return;
            }

            if (Data.Gentagelser < 3)
            {
                Console.WriteLine(
                    "Vi har forsøgt at hente kreditstatus for {0} {1} gange, vi giver den lige et skud mere.....",
                    Data.Cpr, Data.Gentagelser);

                IndhentKreditstatusser();
                return;
            }

            Console.WriteLine(
                "Åh nej dog! Det lader til at vi ikke har modtaget opdaterede kreditoplysninger for {0}..." +
                " Nu kan vi jo så lægge en opgave i opfølgningslisten for den eller de relevante sagsbehandlere" +
                " - #awesomeness!!1",
                Data.Cpr);
        }

        void IndhentKreditstatusser()
        {
            Data.Gentagelser++;

            bus.Send(new IndhentKreditstatusFraRki {Cpr = Data.Cpr});
            bus.Send(new IndhentKreditstatusFraDebitorregisteret {Cpr = Data.Cpr});

            Data.ViAfventerStatussvar = true;
            Data.TidspunktForBestillingAfStatus = DateTime.UtcNow;

            bus.Defer(TimeSpan.FromMinutes(1), new VerificerAtOplysningerErBlevetOpdateret {Cpr = Data.Cpr});
        }

        bool ViHarEnFriskStatus()
        {
            return Data.Statusser.Any(s => s.Hvornår >= DateTime.UtcNow - TimeSpan.FromMinutes(10));
        }
    }

    public class SørgForOpdateredeOplysningerSagaData : ISagaData
    {
        public SørgForOpdateredeOplysningerSagaData()
        {
            Statusser = new List<Kreditstatus>();
        }

        public Guid Id { get; set; }
        public int Revision { get; set; }

        public string Cpr { get; set; }

        public bool ViAfventerStatussvar { get; set; }
        public DateTime TidspunktForBestillingAfStatus { get; set; }
        
        public int Gentagelser { get; set; }

        public List<Kreditstatus> Statusser { get; private set; }
    }

    public class Kreditstatus
    {
        public string Hvorfra { get; set; }
        public bool ThumbsUp { get; set; }
        public DateTime Hvornår { get; set; }
    }
}