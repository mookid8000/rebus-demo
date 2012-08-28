namespace Kreditbandit.Messages
{
    public abstract class IndhentKreditstatus
    {
        public string Cpr { get; set; }
    }

    public class IndhentKreditstatusFraDebitorregisteret : IndhentKreditstatus
    {
    }

    public class IndhentKreditstatusFraRki : IndhentKreditstatus
    {
    }

    public class Kreditstatussvar
    {
        public string Cpr { get; set; }
        public string HvorKomDetFra { get; set; }
        public bool ThumbsUp { get; set; }
    }
}
