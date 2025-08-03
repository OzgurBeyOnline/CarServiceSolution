using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext       _context;
        private readonly IPasswordHasher<Customer>  _hasher;

        public CustomerService(
            ApplicationDbContext context,
            IPasswordHasher<Customer> hasher)
        {
            _context = context;
            _hasher  = hasher;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<int> CountRegisteredInMonthAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end   = start.AddMonths(1);
            return await _context.Customers
                .Where(c => c.CreatedAt >= start && c.CreatedAt < end)
                .CountAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync() =>
            await _context.Customers
                .Include(c => c.City)
                .Where(c => c.Status == AccountStatus.Active)    // sadece aktifleri dön
                .AsNoTracking()
                .ToListAsync();

        public async Task<Customer?> GetByIdAsync(int id) =>
            await _context.Customers
                .Include(c => c.City)
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Customer?> GetByEmailAsync(string email) =>
            await _context.Customers
                .Include(c => c.City)
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(c => c.Email == email);

        public async Task CreateAsync(Customer customer, string plainPassword)
        {
            customer.PasswordHash = _hasher.HashPassword(customer, plainPassword);
            customer.Status       = AccountStatus.Active;
            customer.CreatedAt = DateTime.UtcNow;
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // eski davranış: fiziksel silme yerine soft-delete
            var cust = await _context.Customers.FindAsync(id);
            if (cust != null)
            {
                cust.Status = AccountStatus.Denied;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DenyAsync(int id)
        {
            var cust = await _context.Customers.FindAsync(id);
            if (cust != null)
            {
                cust.Status = AccountStatus.Denied;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivateAsync(int id)
        {
            var cust = await _context.Customers.FindAsync(id);
            if (cust != null)
            {
                cust.Status = AccountStatus.Active;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string plainPassword)
        {
            var user = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Status == AccountStatus.Active);
            if (user == null) return false;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
