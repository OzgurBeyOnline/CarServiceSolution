using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Admin> _hasher;

        public AdminService(
            ApplicationDbContext context,
            IPasswordHasher<Admin> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task<Admin?> GetByEmailAsync(string email) =>
            await _context.Admins
                          .Include(a => a.City)
                          .FirstOrDefaultAsync(a => a.Email == email);

        public async Task<IEnumerable<Admin>> GetAllAsync() =>
            await _context.Admins
                          .Include(a => a.City)
                          .AsNoTracking()
                          .ToListAsync();

        public async Task<Admin?> GetByIdAsync(int id) =>
            await _context.Admins
                          .Include(a => a.City)
                          .FirstOrDefaultAsync(a => a.Id == id);

        public async Task CreateAsync(Admin admin, string plainPassword)
        {
            admin.PasswordHash = _hasher.HashPassword(admin, plainPassword);
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var adm = await _context.Admins.FindAsync(id);
            if (adm != null)
            {
                _context.Admins.Remove(adm);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string plainPassword)
        {
            var user = await _context.Admins
                                     .FirstOrDefaultAsync(a => a.Email == email);
            if (user == null) return false;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
