using fmis.Models.John;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class FundsRealignment
    {
        public int Id { get; set; }
        public int Realignment_from { get; set; }
        public int Realignment_to { get; set; }
        public float Realignment_amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? FundSourceId { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public FundsRealignment()
        {
            this.Updated_At = DateTime.Now;
        }
    }

}
