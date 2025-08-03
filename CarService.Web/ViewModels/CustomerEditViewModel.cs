using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Web.ViewModels
{
    public class CustomerEditViewModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Adınız")]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Soyadınız")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Şehir")]
        public int CityId { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string? CurrentPassword { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }
            = new List<SelectListItem>();
    }
}
