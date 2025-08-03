// Controllers/PricingsController.cs
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Web.Controllers
{
    [Authorize]
    public class PricingsController : Controller
    {
        private readonly IPricingService     _pricingService;
        private readonly IServiceTypeService _serviceTypeService;

        public PricingsController(
            IPricingService     pricingService,
            IServiceTypeService serviceTypeService)
        {
            _pricingService     = pricingService;
            _serviceTypeService = serviceTypeService;
        }

        // GET: /Pricings
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var list = await _pricingService.GetAllAsync();
            return View(list);
        }

        // GET: /Pricings/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var pricing = await _pricingService.GetByIdAsync(id);
            if (pricing == null) 
                return NotFound();

            return View(pricing);
        }

        // GET: /Pricings/Create
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create()
        {
            var vm = new PricingCreateViewModel
            {
                ServiceTypes = (await _serviceTypeService.GetAllAsync())
                  .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                  .ToList()
            };
            return View(vm);
        }

        // POST: /Pricings/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(PricingCreateViewModel vm)
        {
            // repopulate dropdown
            vm.ServiceTypes = (await _serviceTypeService.GetAllAsync())
                .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                .ToList();

            if (!ModelState.IsValid)
                return View(vm);

            await _pricingService.CreateAsync(new Pricing
            {
                ServiceTypeId = vm.ServiceTypeId,
                Price         = vm.Price
            });

            TempData["Success"] = "Yeni fiyat bilgisi başarıyla eklendi.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Pricings/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id)
        {
            var pricing = await _pricingService.GetByIdAsync(id);
            if (pricing == null) 
                return NotFound();

            var vm = new PricingCreateViewModel
            {
                ServiceTypeId = pricing.ServiceTypeId,
                Price         = pricing.Price,
                ServiceTypes  = (await _serviceTypeService.GetAllAsync())
                  .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                  .ToList()
            };
            return View(vm);
        }

        // POST: /Pricings/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, PricingCreateViewModel vm)
        {
            // repopulate dropdown
            vm.ServiceTypes = (await _serviceTypeService.GetAllAsync())
                .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                .ToList();

            if (!ModelState.IsValid)
                return View(vm);

            var pricing = await _pricingService.GetByIdAsync(id);
            if (pricing == null) 
                return NotFound();

            pricing.ServiceTypeId = vm.ServiceTypeId;
            pricing.Price         = vm.Price;
            await _pricingService.UpdateAsync(pricing);

            TempData["Success"] = "Fiyat bilgisi başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Pricings/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pricingService.DeleteAsync(id);
            TempData["Success"] = "Fiyat bilgisi silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
