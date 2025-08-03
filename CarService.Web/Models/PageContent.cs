using System.ComponentModel.DataAnnotations;

namespace CarService.Web.Models
{
    public class PageContent
    {
        public int    Id           { get; set; }
        public string Key          { get; set; } = "";
        public string Html         { get; set; } = "";
        public int    DisplayOrder { get; set; }
    }
}

