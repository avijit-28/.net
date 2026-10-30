using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiProjectMVC.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username or registered email.")]
        [Display(Name = "Username or Email")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
