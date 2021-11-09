using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class SubAllotment_Realignment
    {
        public int Id { get; set; }
        public string Realignment_from { get; set; }
        public string Realignment_to { get; set; }
        public float Realignment_amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int fundsource_id { get; set; }
        public Uacs uacs { get; set; }
    }
}
