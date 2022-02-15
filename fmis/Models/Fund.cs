using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models.silver;

namespace fmis.Models
{
    public class Fund
    {
        [Key]
        public int FundId { get; set; }
        public string Fund_description { get; set; }
        public string Fund_code_current { get; set; }
        public string Fund_code_conap { get; set; }
        public List<FundSource> FundSources { get; set; }
        public List<Sub_allotment> Sub_Allotments { get; set; }
        public List<BudgetAllotment> BudgetAllotments { get; set; }

        [ForeignKey("AppropriationID")]
        public int? AppropriationID { get; set; }
        public Appropriation Appropriation { get; set; }
    }
}
