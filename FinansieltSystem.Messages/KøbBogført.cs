using System.Collections.Generic;

namespace FinansieltSystem.Messages
{
    public class KøbBogført
    {
        public KøbBogført()
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