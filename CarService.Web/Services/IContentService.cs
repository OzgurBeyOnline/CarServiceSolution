using CarService.Web.ViewModels;
using System.Threading.Tasks;
using CarService.Web.Models;
using System.Collections.Generic;

namespace CarService.Web.Services
{
    public interface IContentService
    {
        Task<List<PageContent>>    GetAllAsync();
        Task<PageContent?>         GetByKeyAsync(string key);
        Task UpdateAsync(PageContent content);
        Task UpdateOrderAsync(int[] orderedIds);
    }

}
