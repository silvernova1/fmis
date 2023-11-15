using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fmis.Models.ppmp
{
    public class ItemDaily
    {
        public int Id { get; set; }

        public int Item_id { get; set; }

        [MaxLength(255)]
        public string Unique_id { get; set; }

        [MaxLength(45)]
        public string Userid { get; set; }

        public int? Expense_id { get; set; }

        public int? Division_id { get; set; }

        public int? Section_id { get; set; }

        [MaxLength(45)]
        public string? Tranche { get; set; }

        [MaxLength]
        public string? Code { get; set; }

        [MaxLength]
        public string? Description { get; set; }

        [MaxLength]
        public string? Unit_measurement { get; set; }

        [MaxLength]
        public string? Qty { get; set; }

        [MaxLength]
        public string Unit_cost { get; set; }

        [MaxLength]
        public string? Estimated_budget { get; set; }

        [MaxLength]
        public string? Mode_procurement { get; set; }

        [MaxLength(45)]
        public string? Status { get; set; }

        public int Jan { get; set; }

        public int Feb { get; set; }

        public int Mar { get; set; }

        public int Apr { get; set; }

        public int May { get; set; }

        public int Jun { get; set; }

        public int Jul { get; set; }

        public int Aug { get; set; }

        public int Sep { get; set; }

        public int Oct { get; set; }

        public int Nov { get; set; }

        public int Dece { get; set; }

        public int Yearly_ref_id { get; set; }

        [ForeignKey("Item_id")]
        public Item Item { get; set; }
    }
}
