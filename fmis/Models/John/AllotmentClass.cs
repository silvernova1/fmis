using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class AllotmentClass : BaseEntityTimeStramp
    {
        [Key]
        public int Id { get; set; }
        public string Allotment_Class { get; set; }
        public string Account_Code { get; set; }
        public string Fund_Code { get; set; }
        public string Desc { get; set; }
        public List<BudgetAllotment> BudgetAllotments { get; set; }
        public List<FundSource> FundSource { get; set; }
        public List<SubAllotment> Sub_allotment { get; set; }
        public List<FundSourceTrustFund> FundSourceTrustFund { get; set; }


    }
    
}
