using System;

namespace fmis.Models.Procurement
{
    public class Twg
    {
        public int Id { get; set; }

        public string TwgNo { get; set; }

        public DateTime TwgDate { get; set; }

        public string Prno { get; set; }

        public DateTime PrDate { get; set; }

        public string Recommendation { get; set; }

        public string ReceivedBy { get; set; }
    }
}
