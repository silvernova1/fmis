using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using fmis.Models.Carlo;

namespace fmis.Models
{
    public class SubAllotment  : BaseEntityTimeStramp
    {
        [Key]
        public int SubAllotmentId { get; set; }
        public string Suballotment_title { get; set; }
        public string Description { get; set; }
        public string Suballotment_code { get; set; }
        public bool IsAddToNextAllotment { get; set; }
        public bool Original { get; set; }
        public bool Breakdown { get; set; }
        public string PapType { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [ForeignKey("RespoCenter")]
        public int RespoId { get; set; }
        public RespoCenter RespoCenter { get; set; }

        [ForeignKey("Prexc")]
        public int prexcId { get; set; }
        public Prexc prexc { get; set; }

        [ForeignKey("Appropriation")]
        public int AppropriationId { get; set; }
        public Appropriation Appropriation { get; set; }

        [ForeignKey("AllotmentClass")]
        public int AllotmentClassId { get; set; }
        [JsonIgnore]
        public AllotmentClass AllotmentClass { get; set; }

        [ForeignKey("Fund")]
        public int FundId { get; set; }
        [JsonIgnore]
        public Fund Fund { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Remaining_balance { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Beginning_balance { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal obligated_amount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal utilized_amount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal realignment_amount { get; set; }
        public string token { get; set; }
        public int? BudgetAllotmentId { get; set; }
        public BudgetAllotment Budget_allotment { get; set; }
        public ICollection<Suballotment_amount> SubAllotmentAmounts { get; set; }
        public ICollection<SubAllotment_Realignment> SubAllotmentRealignment { get; set; }
        public ICollection<SubTransferedTo> SubTransferedTo { get; set; }
        public ICollection<Uacs> Uacs { get; set; }
    }

}

