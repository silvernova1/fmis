using System.ComponentModel.DataAnnotations;

namespace fmis.Models.Accounting
{
    public class InfraAdvancePayment
    {
        [Key]
        public int Id { get; set; }
        public decimal? AdvancePayment { get; set; }

        public int? DvId { get; set; }
        public Dv? Dv { get; set; }
    }
}
