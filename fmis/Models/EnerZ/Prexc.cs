using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;

namespace fmis.Models
{
    public class Prexc
    {
        public int Id { get; set; }
        public string pap_title { get; set; }
        public string pap_code1 { get; set; }
        public string pap_code2 { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        
        public FundSource FundSource { get; set; }
    }
}
