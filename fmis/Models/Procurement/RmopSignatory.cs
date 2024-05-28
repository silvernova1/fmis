namespace fmis.Models.Procurement
{
    public class RmopSignatory
    {
        public int RmopSignatoryId { get; set; }

        public int RmopId { get; set; }

        public int RmopAtaId { get; set; }

        public RmopAta RmopAta { get; set; } = null!;
    }
}
