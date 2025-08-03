using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace CarService.Web.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Appointment>> GetAllAsync() =>
            await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.ServiceType)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Appointment?> GetByIdAsync(int id) =>
            await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.ServiceType)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task CreateAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Appointments.CountAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ap = await _context.Appointments.FindAsync(id);
            if (ap != null)
            {
                _context.Appointments.Remove(ap);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IPagedList<Appointment>> GetPagedAsync(int page, int pageSize, string? statusFilter)
        {
            var query = _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.ServiceType)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(statusFilter))
                query = query.Where(a => (a.Status ?? string.Empty).Contains(statusFilter));

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.AppointmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<Appointment>(items, page, pageSize, total);
        }

        public async Task<IPagedList<Appointment>> GetPagedByCustomerAsync(int customerId, int page, int pageSize, string? statusFilter)
        {
            var query = _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .Include(a => a.Employee)
                .Include(a => a.ServiceType)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(statusFilter))
                query = query.Where(a => (a.Status ?? string.Empty).Contains(statusFilter));

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.AppointmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<Appointment>(items, page, pageSize, total);
        }
        public async Task<IEnumerable<Appointment>> GetAllByCustomerAsync(int customerId)
        {
            return await _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .Include(a => a.ServiceType)
                .Include(a => a.Employee)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
