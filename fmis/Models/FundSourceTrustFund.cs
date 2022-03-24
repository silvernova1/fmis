using fmis.Models.Budget;
using fmis.Models.John;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class FundSourceTrustFund
    {
        [Key]
        public int FundSourceTrustFundId { get; set; }
        public string FundSourceTrustFundTitle { get; set; }
        public string Description { get; set; }
        public string FundSourceTrustFundTitleCode { get; set; }
        public string PapType { get; set; }

        [ForeignKey("RespoCenter")]
        public int RespoId { get; set; }
        [JsonIgnore]
        public RespoCenter RespoCenter { get; set; }

        [ForeignKey("PrexcTrustFund")]
        public int PrexcTrustFundId { get; set; }
        [JsonIgnore]
        public PrexcTrustFund PrexcTrustFund { get; set; }

        [ForeignKey("AllotmentClass")]
        public int AllotmentClassId { get; set; }
        [JsonIgnore]
        public AllotmentClass AllotmentClass { get; set; }

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
        public decimal realignment_amount { get; set; }
        public string token { get; set; }
        public int? BudgetAllotmentTrustFundId { get; set; }
        public BudgetAllotmentTrustFund BudgetAllotmentTrustFund { get; set; }
        public ICollection<FundSourceAmountTrustFund> FundSourceAmountTrustFund { get; set; }
        public ICollection<FundsRealignmentTrustFund> FundsRealignmentTrustFund { get; set; }
        public ICollection<UacsTrustFund> UacsTrustFund { get; set; }
    }
}
