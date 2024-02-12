using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.Accounting
{
    public class InfraAdvancePayment
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AdvancePayment { get; set; }

        public int? DvId { get; set; }
        public Dv? Dv { get; set; }
    }
}
