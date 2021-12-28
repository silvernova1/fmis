using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Models
{
    public class Prexc
    {
        public int Id { get; set; }
        public string pap_title { get; set; }
        public string pap_code1 { get; set; }
        public string pap_code2 { get; set; }
        public string status { get; set; }
        public string token { get; set; }

    }
}
