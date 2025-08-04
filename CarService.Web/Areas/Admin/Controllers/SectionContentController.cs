using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Web.Services;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SectionContentController : Controller
    {
        private readonly IJsonContentService _jsonSvc;
        public SectionContentController(IJsonContentService jsonSvc) 
            => _jsonSvc = jsonSvc;

        // ---- Anasayfa ----
        [HttpGet]
        public async Task<IActionResult> EditHome()
        {
            var data = await _jsonSvc.LoadAsync("home");
            return View("EditHome", data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHome(Dictionary<string,string> model)
        {
            if (!ModelState.IsValid) return View("Home", model);
            await _jsonSvc.SaveAsync(model);
            TempData["Success"] = "Anasayfa içerikleri kaydedildi.";
            return RedirectToAction(nameof(EditHome));
        }

        
    }
}
