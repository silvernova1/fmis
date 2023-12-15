using System;

namespace fmis.Models.Procurement
{
    public class PuIndexing
    {
        public int Id { get; set; }

        public string PoNo { get; set; }

        public string PrNo { get; set; }

        public string ItemDesc { get; set; }

        public string Gp { get; set; }

        public string Rmop { get; set; }

        public DateTime BudgetReleased { get; set; }

        public DateTime SupplyReleased { get; set; }

        public string Remarks { get; set; }
    }
}
