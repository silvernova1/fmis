using System;

namespace fmis.Models
{
    public abstract class BaseEntityTimeStramp
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
