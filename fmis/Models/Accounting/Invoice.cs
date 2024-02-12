namespace fmis.Models.Accounting
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int IndexOfPaymentId { get; set; }
        public IndexOfPayment IndexOfPayment { get; set; }
    }
}
