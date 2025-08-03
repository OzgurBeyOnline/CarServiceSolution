using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace CarService.Web.Services
{
    public interface IServiceTypeService
    {
        Task<IEnumerable<ServiceType>> GetAllAsync();
        Task<ServiceType?> GetByIdAsync(int id);
        Task CreateAsync(ServiceType serviceType);
        Task UpdateAsync(ServiceType serviceType);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
        Task<IPagedList<ServiceType>> GetPagedAsync(int page, int pageSize, string? filter);
    }
}
