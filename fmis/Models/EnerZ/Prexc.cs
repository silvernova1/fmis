using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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



    }
}
