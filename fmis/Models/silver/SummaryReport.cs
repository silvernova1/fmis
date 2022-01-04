using fmis.Models.John;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models.Carlo;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.silver
{
    public class SummaryReport
    {
        [Key]
        public int Id { get; set; }
        public string SummaryReports { get; set; }
        [DataType(DataType.Date)]
        public DateTime datefrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateto { get; set; }
        public Uacs Uacs { get; set; }
        public List<FundSource> FundSources { get; set; }
        public List<Prexc> Prexc { get; set; }
    }
}
