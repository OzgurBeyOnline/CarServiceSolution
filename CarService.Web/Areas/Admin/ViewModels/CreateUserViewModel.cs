// ViewModels/CreateUserViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Web.ViewModels
{
    public class CreateUserViewModel
    {
        [Required, Display(Name="Ad")]
        public string FirstName { get; set; } = "";

        [Required, Display(Name="Soyad")]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, Display(Name="E-posta")]
        public string Email { get; set; } = "";

        [Phone, Display(Name="Telefon")]
        public string? PhoneNumber { get; set; }

        [Required, MinLength(8), DataType(DataType.Password), Display(Name="Parola")]
        public string Password { get; set; } = "";

        [Required, Display(Name="Rol")]
        public string Role { get; set; } = "";

        [Required, Display(Name="Şehir")]
        public int CityId { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
    }
}
