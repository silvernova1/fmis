using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.Carlo;

namespace fmis.Models.John
{
    public class FundSource
    {
        [Key]
        public int FundSourceId { get; set; }
        public string FundSourceTitle { get; set; }
        public string Description { get; set; }
        public string FundSourceTitleCode { get; set; }
        public string Respo { get; set; }
        public int Budget_allotmentBudgetAllotmentId { get; set; }
        [ForeignKey("Prexc")]
        public int PrexcId { get; set; }
        public Prexc Prexc { get; set; }
        public decimal Beginning_balance { get; set; }
        public decimal Remaining_balance { get; set; }
        public decimal utilization_amount { get; set; }
        public string token { get; set; }


        public Budget_allotment Budget_allotment { get; set; }
        /*public List<FundSourceAmount> FundSourceAmounts { get; set; }*/



    }
}
