namespace HighlyDeveloped.Core.ViewModels
{
    using DataAnnotationsExtensions;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordViewModel
    {
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter a valid password confirmation")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*d)(?=.*[$@$!%*#?$_])([A-Za-z\d$@$!%*#?&_]){8,}$", ErrorMessage = "Please enter at least 8 characters. Enter a special symbol and a number")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please enter a valid password")]
        [EqualTo("Password", ErrorMessage = "Please ensure your passwords match")]
        public string ConfirmPassword { get; set; }
    }
}