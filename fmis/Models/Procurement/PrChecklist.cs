namespace fmis.Models.Procurement
{
    public class PrChecklist
    {
        public int Id { get; set; }

        public bool IsChecked { get; set; }

        public string Description { get; set; }

        public string Others { get; set; }

        public PuChecklist PuChecklist { get; set; }
    }
}
