using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.pr
{
    public class PrItems
    {
        public int Id { get; set; }

        public int ItemNo { get; set; }

        public string Unit { get; set; }

        public string ItemDescription { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Qty { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal EstUnitCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal EstCost { get; set; }

        public int PrId { get; set; }

        public virtual Pr Pr { get; set; }
    }
}
