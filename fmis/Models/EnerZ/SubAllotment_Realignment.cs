using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class SubAllotment_Realignment
    {
        public int Id { get; set; }
        public int? SubAllotmentAmountId { get; set; } //realignment from
        public int Realignment_to { get; set; }
        public decimal Realignment_amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? SubAllotmentId { get; set; }
        public Suballotment_amount SubAllotmentAmount { get; set; }
        [JsonIgnore]
        public Sub_allotment SubAllotment { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public SubAllotment_Realignment()
        {
            this.Updated_At = DateTime.Now;
        }
    }
}
