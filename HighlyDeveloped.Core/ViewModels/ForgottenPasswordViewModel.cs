namespace HighlyDeveloped.Core.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ForgottenPasswordViewModel
    {
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Please enter a valid email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }
    }
}