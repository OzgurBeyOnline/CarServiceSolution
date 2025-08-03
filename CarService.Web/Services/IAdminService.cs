using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(int id);
        Task CreateAsync(Admin admin, string plainPassword);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(int id);
        Task<Admin?> GetByEmailAsync(string email);
        Task<bool> ValidateCredentialsAsync(string email, string plainPassword);
    }
}
