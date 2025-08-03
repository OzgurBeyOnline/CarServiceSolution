// Areas/Admin/Controllers/ContentController.cs
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContentController : Controller
    {
        private readonly IPageContentService _svc;
        public ContentController(IPageContentService svc) => _svc = svc;

        public async Task<IActionResult> Index()
        {
            var items = await _svc.GetAllAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder([FromBody] int[] orderedIds)
        {
            await _svc.UpdateOrderAsync(orderedIds);
            return Ok();
        }

        public async Task<IActionResult> Edit(string key)
        {
            var item = await _svc.GetByKeyAsync(key);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PageContent vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _svc.UpdateAsync(vm);
            TempData["Success"] = $"{vm.Key} güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
