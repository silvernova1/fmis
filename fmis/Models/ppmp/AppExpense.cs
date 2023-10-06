using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fmis.Models.ppmp
{
    public class AppExpense
    {
        public int Id { get; set; }

        public string? Uacs { get; set; }

        public string? Description { get; set; }

        public List<AppModel>? AppModels { get; set; }
    }
}
