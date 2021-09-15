using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Yearly_reference
    {
        [Key]
        public int Id { get; set; }
        public string YearlyReference { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public Budget_allotment Budget_allotment { get; set; }


    }
}
