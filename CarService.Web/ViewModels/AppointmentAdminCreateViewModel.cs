using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class AppointmentAdminCreateViewModel
    {
        // Şehir seçimi
        [Required]
        [Display(Name = "Şehir")]
        public int CityId { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; } = Array.Empty<SelectListItem>();

        // Müşteri seçimi
        [Required]
        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; } = Array.Empty<SelectListItem>();

        // Çalışan seçimi
        [Required]
        [Display(Name = "Çalışan")]
        public int EmployeeId { get; set; }
        public IEnumerable<SelectListItem> Employees { get; set; } = Array.Empty<SelectListItem>();

        // Çoklu servis tipi seçimi
        [Required]
        [Display(Name = "Servis Tipleri")]
        public List<int> SelectedServiceTypeIds { get; set; } = new List<int>();
        public IEnumerable<SelectListItem> ServiceTypes { get; set; } = Array.Empty<SelectListItem>();

        // Randevu tarihi
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; }

        // İsteğe bağlı not alanı
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
    }
}
