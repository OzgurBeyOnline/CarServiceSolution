using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Display(Name = "Şehir")]
        public int? CityId { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; } = new List<SelectListItem>();

        [Display(Name = "Çalışan")]
        [Required]
        public int EmployeeId { get; set; }      // önceki int? yerine int
        public IEnumerable<SelectListItem> Employees { get; set; } = 
        new List<SelectListItem>();

        [Required, Display(Name = "Servis Tipleri")]
        public List<int> SelectedServiceTypeIds { get; set; } = new();

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Ek Not")]
        public string? Notes { get; set; }

        // Dropdown için
        public IEnumerable<SelectListItem> ServiceTypes { get; set; } = Array.Empty<SelectListItem>();
    }
}
