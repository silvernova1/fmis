using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;


namespace fmis.Models
{
    public class Suballotment_amount
    {
        public int Id { get; set; }
        public int Expenses { get; set; }
        public float Amount { get; set; }
        public int Fund_source { get; set; }
    }
}
