using System.ComponentModel.DataAnnotations;

namespace fmis.Models.Accounting
{
    public class InfraProgress
    {
        [Key]
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public string? bulletNo { get; set; }

        public int? DvId { get; set; }
        public Dv? Dv { get; set; }
    }
}
