using System.ComponentModel.DataAnnotations;

namespace fmis.ViewModel
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "You must enter your username")]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [Required(ErrorMessage = "You must enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
