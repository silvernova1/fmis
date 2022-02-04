using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using fmis.Models.John;
using fmis.Models;

namespace fmis.Models.silver
{
    public class BudgetAllotment : BaseEntityTimeStramp
    {
        [Key]
        public int BudgetAllotmentId { get; set; }
        public string Allotment_series { get; set; }
        public string Allotment_title { get; set; }
        public string Allotment_code { get; set; }
        [JsonIgnore]
        public ICollection<FundSource> FundSources { get; set; }
        [JsonIgnore]
        public ICollection<Sub_allotment> Sub_allotments { get; set; }
        [JsonIgnore]
        public ICollection<Personal_Information> Personal_Information { get; set; }

        [JsonIgnore]
        public ICollection<Appropriation> Appropriation { get; set; }

        [ForeignKey("Yearly_reference")]
        public int YearlyReferenceId { get; set; }
        [JsonIgnore]
        public Yearly_reference Yearly_reference { get; set; }

        [ForeignKey("AllotmentClass")]
        public int AllotmentClassId { get; set; }
        public AllotmentClass AllotmentClass { get; set; }




    }
}
