using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface IJsonContentService
    {
        // Parametresiz: default content.json’i kullanır
        Task<Dictionary<string, string>> LoadAsync();
        Task SaveAsync(Dictionary<string, string> data);

        // Dosya adıyla: "home.json", "about.json", "contact.json" vs.
        Task<Dictionary<string, string>> LoadAsync(string fileName);
        Task SaveAsync(string fileName, Dictionary<string, string> data);
    }
}
