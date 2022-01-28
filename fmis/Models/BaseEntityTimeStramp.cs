using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models
{
    public abstract class BaseEntityTimeStramp
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
