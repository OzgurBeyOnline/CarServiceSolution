using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;
    }
}
