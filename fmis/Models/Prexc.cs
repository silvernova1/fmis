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

        [Display(Name = "Pap Title")]
        public string pap_title { get; set; }

        [Display(Name = "Pap Code 1")]
        public string pap_code1 { get; set; }

        [Display(Name = "Pap Code 2")]
        public string pap_code2 { get; set; }

    }
}
