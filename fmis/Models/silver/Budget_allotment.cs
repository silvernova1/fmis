using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;

namespace fmis.Models
{
    public class Budget_allotment
    {
        [Key]
        public int BudgetAllotmentId { get; set; }
        public string Year { get; set; }
        public string Allotment_series { get; set; }
        public string Allotment_title { get; set; }
        public string Allotment_code { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public List<FundSource> FundSources { get; set; }

        [ForeignKey("Yearly_reference")]
        public int Id { get; set; }
        public Yearly_reference Yearly_reference { get; set; }

    }
}
