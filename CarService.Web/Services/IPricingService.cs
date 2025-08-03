using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface IPricingService
    {
        Task<IEnumerable<Pricing>> GetAllAsync();
        Task<Pricing?> GetByIdAsync(int id);
        Task CreateAsync(Pricing pricing);
        Task UpdateAsync(Pricing pricing);
        Task DeleteAsync(int id);
    }
}
