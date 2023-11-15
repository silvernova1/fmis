using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;

namespace fmis.Models.Procurement
{
    public class PuChecklist
    {
        public int Id { get; set; }

        public int ChecklistNo { get; set; }

        public string Prno { get; set; }

        public DateTime PuChecklistDate { get; set; }

        public List<PrChecklist> PrChecklist { get; set; }
    }
}
