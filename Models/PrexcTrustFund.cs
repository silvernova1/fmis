using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class PrexcTrustFund
    {
        [Key]
        public int PrexcTrustFundId { get; set; }
        public string pap_title { get; set; }
        public string pap_code1 { get; set; }
        public string pap_type { get; set; }
        public string status { get; set; }
        public string token { get; set; }
    }
}
