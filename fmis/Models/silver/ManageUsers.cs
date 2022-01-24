
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.silver
{
    public class ManageUsers
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Email { get; set; }
        [Required(ErrorMessage = "You must enter your username")]
        [Display(Name = "Email")]

        public string Username { get; set; }

        [Required(ErrorMessage = "You must enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember me")]

        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "User Role")]
        public string UserRole { get; set; } 
    }
}
