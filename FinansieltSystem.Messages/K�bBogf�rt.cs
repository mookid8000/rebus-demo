using System.Collections.Generic;

namespace FinansieltSystem.Messages
{
    public class K�bBogf�rt
    {
        public K�bBogf�rt()
        {
            Debitorer = new List<Debitor>();
        }

        public List<Debitor> Debitorer { get; private set; }
    }

    public class Debitor
    {
        public string Cpr { get; set; }
    }
}