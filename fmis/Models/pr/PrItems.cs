namespace fmis.Models.pr
{
    public class PrItems
    {
        public int Id { get; set; }

        public int ItemNo { get; set; }

        public string Unit { get; set; }

        public string ItemDescription { get; set; }

        public decimal Qty { get; set; }

        public decimal EstUnitCost { get; set; }

        public decimal EstCost { get; set; }

        public int PrId { get; set; }

        public virtual Pr Pr { get; set; }
    }
}
