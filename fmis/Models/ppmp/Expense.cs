using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fmis.Models.ppmp
{
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Division { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public string? Uacs { get; set; }

        public List<Item>? Items { get; set; }

    }
}
