// Controllers/ServiceTypesController.cs
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Web.Controllers
{
    [Authorize]
    public class ServiceTypesController : Controller
    {
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IPricingService     _pricingService;

        public ServiceTypesController(
            IServiceTypeService serviceTypeService,
            IPricingService     pricingService)
        {
            _serviceTypeService = serviceTypeService;
            _pricingService     = pricingService;
        }

        // Listeleme: sayfalama + filtre
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, string? filter = null)
        {
            const int PageSize = 10;
            var data = await _serviceTypeService.GetPagedAsync(page, PageSize, filter);
            ViewData["Filter"] = filter;
            return View(data);
        }

        // Detay sayfası
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var st = await _serviceTypeService.GetByIdAsync(id);
            if (st == null) return NotFound();
            return View(st);
        }

        // Yeni servis tipi formu
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
            => View(new ServiceTypeCreateViewModel());

        // Yeni servis tipi ekleme
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(ServiceTypeCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var st = new ServiceType {
                Name        = vm.Name,
                Description = vm.Description ?? string.Empty
            };
            await _serviceTypeService.CreateAsync(st);

            TempData["Success"] = "Yeni servis tipi eklendi.";
            return RedirectToAction("Create", "Pricings");
        }

        // Mevcut servis tipi düzenleme formu
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id)
        {
            var st = await _serviceTypeService.GetByIdAsync(id);
            if (st == null) return NotFound();

            var vm = new ServiceTypeCreateViewModel {
                Name        = st.Name,
                Description = st.Description
            };
            return View(vm);
        }

        // Düzenlenen servisi kaydet
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, ServiceTypeCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var st = await _serviceTypeService.GetByIdAsync(id);
            if (st == null) return NotFound();

            st.Name        = vm.Name;
            st.Description = vm.Description ?? string.Empty;
            await _serviceTypeService.UpdateAsync(st);

            TempData["Success"] = "Servis tipi güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // Silme onayı ekranı
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            var st = await _serviceTypeService.GetByIdAsync(id);
            if (st == null) return NotFound();
            return View(st);
        }

        // Silme işlemi
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceTypeService.DeleteAsync(id);
            TempData["Success"] = "Servis tipi silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
