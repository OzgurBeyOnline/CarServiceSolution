// Areas/Admin/Controllers/AppointmentsController.cs
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee,Admin")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _apptService;

        public AppointmentsController(IAppointmentService apptService)
        {
            _apptService = apptService;
        }

        // Dashboard’daki “Randevu Listele” butonuna gider
        public async Task<IActionResult> AllAppointments(string? searchName)
        {
            var all = await _apptService.GetAllAsync();
            var vm = all.Select(a => new AppointmentListViewModel {
                Id               = a.Id,
                CustomerName     = $"{a.Customer?.FirstName} {a.Customer?.LastName}",
                EmployeeName     = $"{a.Employee?.FirstName} {a.Employee?.LastName}",
                ServiceType      = a.ServiceType?.Name ?? "",
                AppointmentDate  = a.AppointmentDate,
                Price            = a.Price,
                Status           = a.Status
            });

            if (!string.IsNullOrWhiteSpace(searchName))
                vm = vm.Where(x => x.CustomerName
                    .Contains(searchName, StringComparison.OrdinalIgnoreCase));

            ViewData["SearchName"] = searchName;
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _apptService.DeleteAsync(id);
            return RedirectToAction(nameof(AllAppointments));
        }
    }
}
