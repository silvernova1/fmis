using System;

namespace fmis.Models.Procurement
{
    public class Abstract
    {
        public int Id { get; set; }

        public string AbstractNo { get; set; }

        public DateTime AbstractDate { get; set; }

        public string PrNo { get; set; }

        public string PrNoWithDate { get; set; }

        public string CanvassNoWithDate { get; set; }

        public string RecommendedAward { get; set; }

        public string Rmop { get; set; }

		public DateTime PrTrackingDate { get; set; }

		public string Remarks { get; set; }

        public bool IsForBac { get; set; } = false;
	}
}
