using System;

namespace fmis.Models.Procurement
{
    public class Canvass
    {
        public int Id { get; set; }

        public string RpqNo { get; set; }

        public DateTime RpqDate { get; set; }

        public string PrNo { get; set; }

        public DateTime PrDate { get; set; }

        public string ItemDesc { get; set; }

        public string Rmop { get; set; }
    }
}
