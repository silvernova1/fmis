using DocumentFormat.OpenXml.Office.CoverPageProps;
using System.ComponentModel.DataAnnotations;

namespace fmis.Models.Accounting
{
    public class InfraRetention
    {
        [Key]
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public string? billingNo { get; set; }

        public int? DvId { get; set; }
        public Dv? Dv { get; set; }
    }
}
