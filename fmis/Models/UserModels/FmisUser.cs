using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.UserModels
{
    public partial class FmisUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        [NotMapped]
        public string Year { get; set; }
        [NotMapped]
        public int YearId { get; set; }
    }
}
