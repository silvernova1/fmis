using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace fmis.Models.ppmp
{
    public class AppExpense
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public string? Uacs { get; set; }

        public List<AppModel>? AppModels { get; set; }
    }
}
