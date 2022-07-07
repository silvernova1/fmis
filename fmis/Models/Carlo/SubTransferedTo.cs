using fmis.Models.John;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Carlo
{
    public class SubTransferedTo : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int? SubAllotmentAmountId { get; set; } //realignment from
        public int transferedTo { get; set; }
        public decimal Realignment_amount { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? SubAllotmentId { get; set; }
        [JsonIgnore]
        public SubAllotment SubAllotment { get; set; }
        public Suballotment_amount Suballotment_amount { get; set; }

    }
}
