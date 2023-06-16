namespace fmis.Models.Accounting
{
    public class InfraAdvancePayment
    {
        public int Id { get; set; }
        public float AdvancePayment { get; set; }

        public int DvId { get; set; }
        public Dv Dv { get; set; }
    }
}
