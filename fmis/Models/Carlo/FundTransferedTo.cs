using fmis.Models.John;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Carlo
{
    public class FundTransferedTo  : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int? FundSourceAmountId { get; set; } //realignment from
        public int transferedTo { get; set; }
        public decimal Realignment_amount { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? FundSourceId { get; set; }
        [JsonIgnore]
        public FundSource FundSource { get; set; }
        public FundSourceAmount FundSourceAmount { get; set; }

    }
}
