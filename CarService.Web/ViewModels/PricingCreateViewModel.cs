using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CarService.Web.ViewModels
{
    public class PricingCreateViewModel
    {
        public int Id { get; set; }
        
        [Required, Display(Name = "Servis Tipi")]
        public int ServiceTypeId { get; set; }

        [Required, Range(0, double.MaxValue), Display(Name = "Fiyat (TL)")]
        public decimal Price { get; set; }
        public IEnumerable<SelectListItem> ServiceTypes { get; set; } = new List<SelectListItem>();
    }
}
