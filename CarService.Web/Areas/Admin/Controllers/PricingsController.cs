// Areas/Admin/Controllers/PricingsController.cs
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class PricingsController : Controller
    {
        private readonly IPricingService     _pricing;
        private readonly IServiceTypeService _svcType;

        public PricingsController(
            IPricingService pricingService,
            IServiceTypeService serviceTypeService)
        {
            _pricing = pricingService;
            _svcType = serviceTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _pricing.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new PricingCreateViewModel {
                ServiceTypes = (await _svcType.GetAllAsync())
                  .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
            };
            var allPricings = await _pricing.GetAllAsync();
            ViewBag.AllPricings = allPricings;
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PricingCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ServiceTypes = (await _svcType.GetAllAsync())
                    .Select(st => new SelectListItem(st.Name, st.Id.ToString()));
                return View(vm);
            }
            await _pricing.CreateAsync(new Pricing {
                ServiceTypeId = vm.ServiceTypeId,
                Price         = vm.Price
            });
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Edit(int id)
        {
            
            var p = await _pricing.GetByIdAsync(id);
            if (p == null) return NotFound();
            var vm = new PricingCreateViewModel {
                Id            = p.Id,
                ServiceTypeId = p.ServiceTypeId,
                Price         = p.Price,
                ServiceTypes  = (await _svcType.GetAllAsync())
                    .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PricingCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ServiceTypes = (await _svcType.GetAllAsync())
                  .Select(st => new SelectListItem(st.Name, st.Id.ToString()));
                return View(vm);
            }
            var p = await _pricing.GetByIdAsync(vm.Id);
            if (p == null) return NotFound();
            p.ServiceTypeId = vm.ServiceTypeId;
            p.Price         = vm.Price;
            await _pricing.UpdateAsync(p);
            return RedirectToAction(nameof(Create));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _pricing.DeleteAsync(id);
            return RedirectToAction(nameof(Create));
        }
    }
}
