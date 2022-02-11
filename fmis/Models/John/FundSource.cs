using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.silver;

namespace fmis.Models.John
{
    public class FundSource : BaseEntityTimeStramp
    {
        [Key]
        public int FundSourceId { get; set; }
        public string FundSourceTitle { get; set; }
        public string Description { get; set; }
        public string FundSourceTitleCode { get; set; }

        [ForeignKey("RespoCenter")]
        public int RespoId { get; set; }
        [JsonIgnore]
        public RespoCenter RespoCenter { get; set; }

        [ForeignKey("Prexc")]
        public int PrexcId { get; set; }
        [JsonIgnore]
        public Prexc Prexc { get; set; }

        [ForeignKey("Appropriation")]
        public int AppropriationId { get; set; }
        [JsonIgnore]
        public Appropriation Appropriation { get; set; }

        [ForeignKey("Fund")]
        public int FundId { get; set; }
        [JsonIgnore]
        public Fund Fund { get; set; }


        public decimal Beginning_balance { get; set; }
        public decimal Remaining_balance { get; set; }
        public decimal obligated_amount { get; set; }
        public decimal utilized_amount { get; set; }
        public decimal realignment_amount { get; set; }
        public string token { get; set; }
        public int? BudgetAllotmentId { get; set; }
        public BudgetAllotment BudgetAllotment { get; set; }
        public ICollection<FundSourceAmount> FundSourceAmounts { get; set; }
        public ICollection<FundsRealignment> FundsRealignment { get; set; }


        public ICollection<Uacs> Uacs { get; set; }

    }
}
