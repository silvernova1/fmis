using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models;
using System.Text.Json.Serialization;

namespace fmis.Models
{
    public class Budget_allotment
    {
        [Key]
        public int BudgetAllotmentId { get; set; }
        public string Allotment_series { get; set; }
        public string Allotment_title { get; set; }
        public string Allotment_code { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        [JsonIgnore]
        public ICollection<FundSource> FundSources { get; set; }
        [JsonIgnore]
        public ICollection<Sub_allotment> Sub_allotments { get; set; }
        [JsonIgnore]
        public ICollection<Personal_Information> Personal_Information { get; set; }

        [ForeignKey("Yearly_reference")]
        public int YearlyReferenceId { get; set; }
        [JsonIgnore]
        public Yearly_reference Yearly_reference { get; set; }


        //ADDITIONAL FIELDS BASED ON OLD BUDGET SYSTEM

        public String year { get; set; }
        public Int32 active { get; set; }
        public String Code2 { get; set; }

        public Budget_allotment()
        {
            this.Updated_at = DateTime.Now;
        }

    }
}
