using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "İsim gerekli.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyisim gerekli.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mesaj alanı boş olamaz.")]
        public string Message { get; set; } = string.Empty;
    }
}
