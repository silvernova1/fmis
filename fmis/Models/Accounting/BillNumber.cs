namespace fmis.Models.Accounting
{
    public class BillNumber
    {
        public int Id { get; set; }
        public int? NumberOfBilling { get; set; }

        public int IndexOfPaymentId { get; set; }
        public virtual IndexOfPayment IndexOfPayment { get; set; }

    }
}
