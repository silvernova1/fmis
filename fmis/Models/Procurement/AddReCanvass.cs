using System;
using System.Collections.Generic;

namespace fmis.Models.Procurement
{
    public class AddReCanvass
    {
        public int AddReCanvassId { get; set; }

        public string RpqNo { get; set; }

        public DateTime RpqDate { get; set; }

        public string PrNo { get; set; }

        public DateTime PrDate { get; set; }

        public string ItemDesc { get; set; }

        public string Rmop { get; set; }

        public DateTime PrTrackingDate { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string Remarks { get; set; }

        public string Step { get; set; }

        public int? CanvassId { get; set; }

        public Canvass Canvass { get; set; }


    }
}
