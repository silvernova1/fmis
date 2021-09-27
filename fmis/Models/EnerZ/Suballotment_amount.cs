using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Models
{
    public class Suballotment_amount
    {
        [Key]
        public int Id { get; set; }
        public int Expenses { get; set; }
        public float Amount { get; set; }
        public int Fund_source { get; set; }

        public Sub_allotment Sub_allotment { get; set; }

    }
}
