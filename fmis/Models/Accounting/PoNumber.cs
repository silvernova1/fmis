namespace fmis.Models.Accounting
{
    public class PoNumber
    {
        public int PoNumberId { get; set; }

        public string PoNum { get; set; }

        public int IndexOfPaymentId { get; set; }

        public IndexOfPayment IndexOfPayment { get; set; }
    }
}
