using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class FundsRealignmentTrustFund
    {
        public int Id { get; set; }
        public int? FundSourceAmountTrustFundId { get; set; } //realignment from
        public int RealignmentTo { get; set; }
        public decimal RealignmentAmount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? FundSourceId { get; set; }
        [JsonIgnore]
        public FundSourceTrustFund FundSourceTrustFund { get; set; }
        public FundSourceAmountTrustFund FundSourceAmountTrustFund { get; set; }

    }
}
