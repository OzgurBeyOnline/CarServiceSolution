using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class AppointmentEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Servis Tipi")]
        public int ServiceTypeId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Notlar")]
        public string Notes { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> ServiceTypes { get; set; } = Array.Empty<SelectListItem>();
    }
}
