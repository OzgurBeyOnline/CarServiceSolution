using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace CarService.Web.Services
{
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly ApplicationDbContext _context;
        public ServiceTypeService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<ServiceType>> GetAllAsync() =>
            await _context.ServiceTypes
                .Include(st => st.Pricings)
                .AsNoTracking()
                .ToListAsync();

        public async Task<ServiceType?> GetByIdAsync(int id) =>
            await _context.ServiceTypes
                .Include(st => st.Pricings)
                .Include(st => st.Pricings)
                .Include(st => st.Appointments)
                .FirstOrDefaultAsync(st => st.Id == id);

        public async Task CreateAsync(ServiceType serviceType)
        {
            await _context.ServiceTypes.AddAsync(serviceType);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.ServiceTypes.CountAsync();
        }

        public async Task UpdateAsync(ServiceType serviceType)
        {
            _context.ServiceTypes.Update(serviceType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var st = await _context.ServiceTypes.FindAsync(id);
            if (st != null)
            {
                _context.ServiceTypes.Remove(st);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IPagedList<ServiceType>> GetPagedAsync(int page, int pageSize, string? filter)
        {
            var query = _context.ServiceTypes
                                .Include(st => st.Pricings)
                                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter))
                query = query.Where(st => st.Name.Contains(filter));

            // Toplam kayıt sayısını al
            var totalCount = await query.CountAsync();

            // İlgili sayfayı çek
            var items = await query
                .OrderBy(st => st.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // StaticPagedList ile paketle
            return new StaticPagedList<ServiceType>(items, page, pageSize, totalCount);
        }
    }
}
