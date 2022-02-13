using System.Collections.Generic;

namespace fmis.Models.Budget
{
    public class PapType
    {
        public int PapTypeID { get; set; }
        public string PapTypeName { get; set; }

        public ICollection<Prexc> Prexcs { get; set; }
    }
}
