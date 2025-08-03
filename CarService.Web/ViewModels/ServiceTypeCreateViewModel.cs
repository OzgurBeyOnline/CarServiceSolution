using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class ServiceTypeCreateViewModel
    {
        [Required, Display(Name = "Servis Adı")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}
