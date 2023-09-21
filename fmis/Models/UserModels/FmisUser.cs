using fmis.Data;
using fmis.Models.Accounting;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace fmis.Models.UserModels
{
    public partial class FmisUser : EmployeeModel
    {

        public int Id { get; set; }
        //[Remote("IsUsernameUnique", "Users", ErrorMessage = "This username is already in use.")]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }


        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public string Year { get; set; }
        [NotMapped]
        public int YearId { get; set; }
    }
}
