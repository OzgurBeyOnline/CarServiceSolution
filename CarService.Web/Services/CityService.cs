using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;
        public CityService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<City>> GetAllAsync() =>
            await _context.Cities.AsNoTracking().ToListAsync();

        public async Task<City?> GetByIdAsync(int id) =>
            await _context.Cities
                .Include(c => c.Customers)
                .Include(c => c.Employees)
                .Include(c => c.Admins)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task CreateAsync(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
        }
    }
}
