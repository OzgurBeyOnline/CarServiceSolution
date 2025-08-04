using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Web.Services;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactContentController : Controller
    {
        private readonly IJsonContentService _jsonSvc;
        private const string _file = "contact.json";

        public ContactContentController(IJsonContentService jsonSvc) => _jsonSvc = jsonSvc;

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var data = await _jsonSvc.LoadAsync("contact.json");
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dictionary<string,string> model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _jsonSvc.SaveAsync("contact", model);
            TempData["Success"] = " Footer İletişim içeriği güncellendi.";
            return RedirectToAction(nameof(Edit));
        }
    }
}
