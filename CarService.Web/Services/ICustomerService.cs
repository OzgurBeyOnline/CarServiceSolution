using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task CreateAsync(Customer customer, string plainPassword);
        Task UpdateAsync(Customer customer);
        Task<Customer?> GetByEmailAsync(string email);
        Task DeleteAsync(int id);
        Task DenyAsync(int id);
        Task<int> CountAsync();
        Task<int> CountRegisteredInMonthAsync(int year, int month);
        Task ActivateAsync(int id);
        Task<bool> ValidateCredentialsAsync(string email, string plainPassword);
    }
}
