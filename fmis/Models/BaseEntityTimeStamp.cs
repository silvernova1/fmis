using System;

namespace fmis.Models
{
    public abstract class BaseEntityTimeStamp
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
