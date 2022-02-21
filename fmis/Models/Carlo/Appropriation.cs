using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models.silver;

namespace fmis.Models
{
    public class Appropriation : BaseEntityTimeStramp
    {
        public int AppropriationId { get; set; }
        public string AppropriationSource { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public List<FundSource> FundSources { get; set; }
        public List<BudgetAllotment> BudgetAllotments { get; set; }

        public virtual ICollection<Fund> Funds { get; set; }
    }
}
