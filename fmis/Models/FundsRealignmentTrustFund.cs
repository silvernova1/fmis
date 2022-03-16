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
        public int Realignment_to { get; set; }
        public decimal Realignment_amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? FundSourceTrustFundId { get; set; }
        [JsonIgnore]
        public FundSourceTrustFund FundSourceTrustFund { get; set; }
        public FundSourceAmountTrustFund FundSourceAmountTrustFund { get; set; }

    }
}
