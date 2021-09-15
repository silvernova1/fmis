using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class FundSourceAmount
    {
        [Key]
        public int FundsId { get; set; }
        public string Amount { get; set; }

    }
}
