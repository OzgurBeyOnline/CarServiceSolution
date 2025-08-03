using System.ComponentModel.DataAnnotations;

namespace CarService.Web.ViewModels
{
    public class ContentViewModel
    {
        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Html)]
        public string Html { get; set; } = string.Empty;
    }
}
