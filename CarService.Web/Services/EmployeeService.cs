using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class EmployeeService : IEmployeeService
    {
        public async Task<IEnumerable<Employee>> GetByCityAsync(int cityId)
        {
            return await _context.Employees
                .Where(e => e.CityId == cityId)
                .ToListAsync();
        }
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Employee> _hasher;

        public EmployeeService(
            ApplicationDbContext context,
            IPasswordHasher<Employee> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<Employee?> GetByEmailAsync(string email) =>
            await _context.Employees
                          .Include(e => e.City)
                          .FirstOrDefaultAsync(e => e.Email == email);

        public async Task<IEnumerable<Employee>> GetAllAsync() =>
            await _context.Employees
                          .Include(e => e.City)
                          .AsNoTracking()
                          .ToListAsync();

        public async Task<Employee?> GetByIdAsync(int id) =>
            await _context.Employees
                          .Include(e => e.City)
                          .Include(e => e.Appointments)
                          .FirstOrDefaultAsync(e => e.Id == id);

        // Artık plainPassword parametresi de alıyor ve hash’liyor
        public async Task CreateAsync(Employee employee, string plainPassword)
        {
            employee.PasswordHash = _hasher.HashPassword(employee, plainPassword);
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp != null)
            {
                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();
            }
        }

        // Email + düz metin parola ile kimlik doğrulama
        public async Task<bool> ValidateCredentialsAsync(string email, string plainPassword)
        {
            var user = await _context.Employees
                                     .FirstOrDefaultAsync(e => e.Email == email);
            if (user == null) 
                return false;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
