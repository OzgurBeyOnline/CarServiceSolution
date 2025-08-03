using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class PricingService : IPricingService
    {
        private readonly ApplicationDbContext _context;
        public PricingService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Pricing>> GetAllAsync() =>
            await _context.Pricings
                .Include(p => p.ServiceType)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Pricing?> GetByIdAsync(int id) =>
            await _context.Pricings
                .Include(p => p.ServiceType)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task CreateAsync(Pricing pricing)
        {
            await _context.Pricings.AddAsync(pricing);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pricing pricing)
        {
            _context.Pricings.Update(pricing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pr = await _context.Pricings.FindAsync(id);
            if (pr != null)
            {
                _context.Pricings.Remove(pr);
                await _context.SaveChangesAsync();
            }
        }
    }
}
