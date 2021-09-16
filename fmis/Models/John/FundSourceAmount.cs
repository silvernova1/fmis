using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class FundSourceAmount
    {
        [Key]
        public int Id { get; set; }
        public int FundSourceId { get; set; }
        public float Amount { get; set; }
    }
}
