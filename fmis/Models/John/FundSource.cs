using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.silver;

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
        [ForeignKey("Prexc")]
        public int PrexcId { get; set; }
        public Prexc Prexc { get; set; }
        public decimal Beginning_balance { get; set; }
        public decimal Remaining_balance { get; set; }
        public decimal obligated_amount { get; set; }
        public decimal realignment_amount { get; set; }
        public string token { get; set; }
        public int? BudgetAllotmentId { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public BudgetAllotment BudgetAllotment { get; set; }
        public ICollection<FundSourceAmount> FundSourceAmounts { get; set; }
        public ICollection<FundsRealignment> FundsRealignment { get; set; }
        public ICollection<Uacs> Uacs { get; set; }

        public FundSource()
        {
            this.Updated_At = DateTime.Now;
        }
    }
}
