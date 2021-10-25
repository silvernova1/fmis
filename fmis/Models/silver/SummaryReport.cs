using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.silver
{
    public class SummaryReport
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime datefrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateto { get; set; }
    }
}
