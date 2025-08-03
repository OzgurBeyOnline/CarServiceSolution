using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Adınız gerekli")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyadınız gerekli")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası gerekli")]
        [Phone]
        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gerekli")]
        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola gerekli")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola tekrarı gerekli")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor")]
        [Display(Name = "Parola (Tekrar)")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir seçmelisiniz")]
        [Display(Name = "Yaşadığınız Şehir")]
        public int CityId { get; set; }

        // dropdown
        public IEnumerable<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public bool RememberMe { get; set; } = false;
    }
}
