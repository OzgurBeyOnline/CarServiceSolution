// Areas/Admin/Controllers/ServiceTypesController.cs
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class ServiceTypesController : Controller
    {
        private readonly IServiceTypeService _svc;

        public ServiceTypesController(IServiceTypeService svc) => _svc = svc;

        public async Task<IActionResult> Index()
        {
            var list = await _svc.GetAllAsync();
            return View(list);
        }

        public IActionResult Create() => View(new ServiceTypeCreateViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceTypeCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _svc.CreateAsync(new ServiceType {
                Name = vm.Name,
                Description = vm.Description ?? string.Empty
            });
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var st = await _svc.GetByIdAsync(id);
            if (st == null) return NotFound();
            var vm = new ServiceTypeCreateViewModel {
                Name = st.Name,
                Description = st.Description
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceTypeCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var st = await _svc.GetByIdAsync(id);
            if (st == null) return NotFound();
            st.Name = vm.Name;
            st.Description = vm.Description ?? string.Empty;
            await _svc.UpdateAsync(st);
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var st = await _svc.GetByIdAsync(id);
            if (st == null) return NotFound();
            return View(st);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _svc.DeleteAsync(id);
            return RedirectToAction(nameof(Create));
        }
    }
}
