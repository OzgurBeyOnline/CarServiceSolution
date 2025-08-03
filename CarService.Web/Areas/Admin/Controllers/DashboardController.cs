// Controllers/DashboardController.cs
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace CarService.Web.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceTypeService _serviceTypeService;

        public DashboardController(
            ICustomerService customerService,
            IEmployeeService employeeService,
            IAppointmentService appointmentService,
            IServiceTypeService serviceTypeService)
        {
            _customerService = customerService;
            _employeeService = employeeService;
            _appointmentService = appointmentService;
            _serviceTypeService = serviceTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TotalCustomers = await _customerService.CountAsync(),
                TotalEmployees = await _employeeService.CountAsync(),
                TotalAppointments = await _appointmentService.CountAsync(),
                TotalServices = await _serviceTypeService.CountAsync()
            };

            var today = DateTime.Today;
            var labels = Enumerable.Range(0, 6)
                .Select(i => today.AddMonths(-i).ToString("MMM yyyy"))
                .Reverse()
                .ToList();

            vm.MonthLabels = labels;

            var custPerMonth = labels
                .Select(label =>
                {
                    var date = DateTime.ParseExact(label, "MMM yyyy", null);
                    return _customerService
                       .CountRegisteredInMonthAsync(date.Year, date.Month)
                       .Result;
                })
                .ToList();
            vm.NewCustomersPerMonth = custPerMonth;

            // 3) Hizmet tipi dağılımı
            var services = await _serviceTypeService.GetAllAsync();
            vm.ServiceTypeLabels = services.Select(s => s.Name).ToList();
            vm.ServiceTypeCounts = services
                .Select(s => s.Appointments?.Count ?? 0)
                .ToList();

            // 4) En aktif çalışan & en çok randevu alan müşteri
            var appts = await _appointmentService.GetAllAsync();
            var topEmp = appts
                .GroupBy(a => a.EmployeeId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();
            if (topEmp != null)
            {
                var emp = await _employeeService.GetByIdAsync(topEmp.Key);
                vm.TopEmployeeName = emp != null
                    ? $"{emp.FirstName} {emp.LastName}"
                    : "—";
                vm.TopEmployeeAppointments = topEmp.Count();
            }

            var topCust = appts
                .GroupBy(a => a.CustomerId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();
            if (topCust != null)
            {
                var cust = await _customerService.GetByIdAsync(topCust.Key);
                vm.TopCustomerName = cust != null
                    ? $"{cust.FirstName} {cust.LastName}"
                    : "—";
                vm.TopCustomerAppointments = topCust.Count();
            }
            return View(vm);
        }
        
        [HttpGet]
        public async Task<FileResult> ExportCustomers()
        {
            // Şehir bilgisini de çekmek için GetAllAsync() içinde Include(c => c.City) olduğundan emin olun
            var list = await _customerService.GetAllAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,FirstName,LastName,Email,PhoneNumber,City");

            foreach (var c in list)
            {
                var city = c.City?.Name?.Replace(",", " ") ?? string.Empty;
                sb.AppendLine($"{c.Id},{c.FirstName},{c.LastName},{c.Email},{c.PhoneNumber},{city}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            // CSV olarak gönderiyoruz; Excel’de açmak için uygun
            return File(bytes, "text/csv", "customers.csv");
        }
    }
}
