using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Budget_allotment
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string Allotment_series { get; set; }
        public string Allotment_title { get; set; }
        public string Allotment_code { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

    }
}
