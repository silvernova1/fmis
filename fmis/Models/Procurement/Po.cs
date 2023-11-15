using System;

namespace fmis.Models.Procurement
{
    public class Po
    {
        public int Id { get; set; }

        public string PoNo { get; set; }

        public DateTime PoDate { get; set; }

        public string PrNo { get; set; }

        public DateTime PrDate { get; set; }

        public string Description { get; set; }

        public string Supplier { get; set; }

        public decimal Amount { get; set; }

        public string Rmop { get; set; }

        public string Remarks { get; set; }
    }
}
