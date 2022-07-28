using fmis.Models.John;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Carlo
{
    public class SubTransferedTo : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int? SubAllotmentAmountId { get; set; } //realignment from
        public string Particulars { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? SubAllotmentId { get; set; }
        [JsonIgnore]
        public SubAllotment SubAllotment { get; set; }

    }
}
