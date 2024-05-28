using System;

namespace fmis.Models.Procurement
{
    public class PoList
    {
        public int Id { get; set; }

        public string PoNo { get; set; }

        public DateTime PoDate { get; set; }

        public string Supplier { get; set; }

        public decimal Amount { get; set; }

        public int PoId { get; set; }

        public Po Po { get; set; } = null!;
    }
}
