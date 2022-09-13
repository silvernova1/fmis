using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Sections
    {
        [Key]
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string Description { get; set; }
        public string SectionHead { get; set; }

        [ForeignKey("RespoCenter")]
        public int RespoId { get; set; }
        [JsonIgnore]
        public RespoCenter RespoCenter { get; set; }
    }
}
