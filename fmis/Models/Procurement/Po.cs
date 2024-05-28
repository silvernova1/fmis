using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace fmis.Models.Procurement
{
    public class Po
    {
        public int Id { get; set; }

        public string AbstractNo { get; set; }

        public string PoNo { get; set; }

        public DateTime PoDate { get; set; }

        public string PrNo { get; set; }

        public DateTime PrDate { get; set; }

        public string Description { get; set; }

        public string Supplier { get; set; }

        public decimal Amount { get; set; }

        public string Rmop { get; set; }

        public string Remarks { get; set; }

		public DateTime PrTrackingDate { get; set; }

        public bool IsForBudget { get; set; } = false;

        public ICollection<PoList> PoList { get; set; }
    }
}
