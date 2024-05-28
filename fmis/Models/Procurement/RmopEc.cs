namespace fmis.Models.Procurement
{
    public class RmopEc
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public string BacNo { get; set; }

        public string PrNoOne { get; set; }

        public string PrDescriptionOne { get; set; }

        public decimal PrAmountOne { get; set; }

        public string PrNoTwo { get; set; }

        public decimal PrAmountTwo { get; set; }

        public string PrDescriptionTwo { get; set; }

        public string PrDate { get; set; }

        public bool IsForBac { get; set; } = false;

        public bool IsForRd { get; set; } = false;

        public string? Remarks { get; set; }
    }
}
