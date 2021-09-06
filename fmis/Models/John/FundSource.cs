using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Models.John
{
    public class FundSource
    {
        [Key]
        public int FundSourceId { get; set; }
        public string PrexcCode { get; set; }
        public string FundSourceTitle { get; set; }
        public string Description { get; set; }
        public string FundSourceTitleCode { get; set; }
        public string Respo { get; set; }
        public int Budget_allotmentBudgetAllotmentId { get; set; }


        public Budget_allotment Budget_allotment { get; set; }


    }
}
