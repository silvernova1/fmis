using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.Budget
{
    public class PapType
    {
        public int PapTypeID { get; set; }
        public string PapTypeName { get; set; }

        [NotMapped]
        public int Id { get; set; }
    }
}
