using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fmis.ViewModel
{
    public class LoginViewModel
    {


        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = false;

        public string ReturnUrl { get; set; }

        public int Year { get; set; }
    }
}
