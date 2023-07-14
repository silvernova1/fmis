using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.Carlo
{
    public class SubNegative : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int? SubAllotmentAmountId { get; set; } //realignment from
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? SubAllotmentId { get; set; }
        [JsonIgnore]
        public SubAllotment SubAllotment { get; set; }
    }
}
