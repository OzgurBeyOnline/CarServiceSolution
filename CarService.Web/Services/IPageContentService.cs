// Services/IPageContentService.cs
using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface IPageContentService
    {
        Task<IList<PageContent>> GetAllAsync();
        Task<PageContent?> GetByKeyAsync(string key);
        Task UpdateAsync(PageContent entity);
        Task UpdateOrderAsync(int[] orderedIds);
        // … gerekirse Create/Delete metotları da
    }
}
