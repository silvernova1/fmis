using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class FundSourceAmountTrustFund
    {
        [Key]
        public int FundSourceAmountTrustFundId { get; set; }
        public int UacsId { get; set; }
        public decimal BeginningBalance { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal RealignmentAmount { get; set; }
        public string status { get; set; }
        public string FundSourceAmountTokenTrustFund { get; set; }
        public string FundSourceTokenTrustFund { get; set; }
        public int BudgetAllotmentTrustFundId { get; set; }
        public int? FundSourceTrustFundId { get; set; }
        [JsonIgnore]
        public FundSourceTrustFund FundSourceTrustFund { get; set; }
        public UacsTrustFund UacsTrustFund { get; set; }
    }
}
