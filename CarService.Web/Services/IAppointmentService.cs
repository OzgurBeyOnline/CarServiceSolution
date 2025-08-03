using CarService.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace CarService.Web.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IPagedList<Appointment>> GetPagedAsync(int page, int pageSize, string? statusFilter);
        Task<IPagedList<Appointment>> GetPagedByCustomerAsync(int customerId, int page, int pageSize, string? statusFilter);
        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task<int> CountAsync();
        Task<IEnumerable<Appointment>> GetAllByCustomerAsync(int customerId);
        Task DeleteAsync(int id);
    }
}
