using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Section
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int Division { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public string Head { get; set; }
        public string Code { get; set; }
        public string Remember_Token { get; set; }
        [DataType(DataType.Date)]
        public DateTime Created_At { get; set; }
        [DataType(DataType.Date)]
        public DateTime Updated_At { get; set; }
    }
}
