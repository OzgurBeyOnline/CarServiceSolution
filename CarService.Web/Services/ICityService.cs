using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task CreateAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(int id);
    }
}
