using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class EmployeeEditViewModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, Display(Name = "Şehir")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
    }
}
