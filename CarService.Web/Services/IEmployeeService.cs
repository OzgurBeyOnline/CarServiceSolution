using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetByCityAsync(int cityId);
        Task CreateAsync(Employee employee, string plainPassword);
        Task UpdateAsync(Employee employee);
        Task<Employee?> GetByEmailAsync(string email);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
        Task<bool> ValidateCredentialsAsync(string email, string plainPassword);
    }
}
