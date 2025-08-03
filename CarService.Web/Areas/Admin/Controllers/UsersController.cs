// Areas/Admin/Controllers/UsersController.cs
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using DomainAdmin = global::CarService.Web.Models.Admin;

namespace CarService.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IAccountService          _accountSvc;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomerService         _custSvc;
        private readonly IEmployeeService         _empSvc;
        private readonly IAdminService            _admSvc;
        private readonly ICityService             _citySvc;

        public UsersController(
            UserManager<IdentityUser> userManager,
            ICustomerService custSvc,
            IEmployeeService empSvc,
            IAdminService admSvc,
            ICityService citySvc,
            IAccountService accountSvc)
        {
            _userManager  = userManager;
            _custSvc      = custSvc;
            _empSvc       = empSvc;
            _admSvc       = admSvc;
            _citySvc      = citySvc;
            _accountSvc   = accountSvc;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(string? search, string? roleFilter)
        {
            ViewData["Search"]     = search;
            ViewData["RoleFilter"] = roleFilter;
            // 1) Tüm AspNetUsers
            var identityUsers = (await _accountSvc.GetAllUsersAsync()).ToList();

            // 2) İsim veya email filtresi uygula (varsa)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                identityUsers = identityUsers
                    .Where(u =>
                        (u.Email?.ToLower().Contains(q) ?? false) 
                        || (u.UserName?.ToLower().Contains(q) ?? false)
                    )
                    .ToList();
            }

            // 3) Role filtresi uygula (varsa)
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                var tmp = new List<IdentityUser>();
                foreach (var u in identityUsers)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    if (roles.Contains(roleFilter))
                        tmp.Add(u);
                }
                identityUsers = tmp;
            }

            // 4) ViewModel’e dönüştür (her bir tabloya sorup ad soyadı alın)
            var model = new List<UserSummaryViewModel>();
            foreach (var u in identityUsers)
            {
                // Kullanıcı hangi tabloya ait? (Customer/Employee/Admin)
                string first = "", last = "";
                var roles = await _userManager.GetRolesAsync(u);
                if (roles.Contains("Customer"))
                {
                    var cust = await _custSvc.GetByEmailAsync(u.Email!);
                    first = cust?.FirstName ?? "";
                    last  = cust?.LastName  ?? "";
                }
                else if (roles.Contains("Employee"))
                {
                    var emp = await _empSvc.GetByEmailAsync(u.Email!);
                    first = emp?.FirstName ?? "";
                    last  = emp?.LastName  ?? "";
                }
                else if (roles.Contains("Admin"))
                {
                    var adm = await _admSvc.GetByEmailAsync(u.Email!);
                    first = adm?.FirstName ?? "";
                    last  = adm?.LastName  ?? "";
                }

                model.Add(new UserSummaryViewModel {
                    Id         = u.Id,
                    Email      = u.Email ?? "",
                    Role       = roles.FirstOrDefault() ?? "",
                    FirstName  = first,
                    LastName   = last
                });
            }

            // 5) View'a ver
            return View(model);
        }

        // GET: /Admin/Users/CreateUser
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Roles  = new[] { "Admin", "Employee", "Customer" };
            ViewBag.Cities = (await _citySvc.GetAllAsync())
                     .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                     .ToList();
            return View(new CreateUserViewModel());
        }

        // POST: /Admin/Users/CreateUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel vm)
        {
            ViewBag.Roles  = new[] { "Admin", "Employee", "Customer" };
            ViewBag.Cities = (await _citySvc.GetAllAsync())
                     .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                     .ToList();

            if (!ModelState.IsValid)
                return View(vm);

            // 1) IdentityUser kaydı
            var reg = await _accountSvc.RegisterAsync(vm.Email, vm.Password);
            if (!reg.Succeeded)
            {
                foreach (var e in reg.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            // 2) Role ataması
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı oluşturulamadı.");
                return View(vm);
            }
            await _userManager.AddToRoleAsync(user, vm.Role);

            // 3) Domain tablosuna ekle
            switch (vm.Role)
            {
                case "Customer":
                    var cust = new Customer
                    {
                        FirstName    = vm.FirstName,
                        LastName     = vm.LastName,
                        Email        = vm.Email,
                        PhoneNumber  = vm.PhoneNumber ?? string.Empty,
                        CityId       = vm.CityId,
                        Status       = AccountStatus.Active,
                        CreatedAt    = System.DateTime.UtcNow,
                        PasswordHash = user.PasswordHash ?? string.Empty
                    };
                    await _custSvc.CreateAsync(cust, vm.Password);
                    break;

                case "Employee":
                    var emp = new Employee
                    {
                        FirstName    = vm.FirstName,
                        LastName     = vm.LastName,
                        Email        = vm.Email,
                        PhoneNumber  = vm.PhoneNumber ?? string.Empty,
                        CityId       = vm.CityId,
                        Status       = AccountStatus.Active,
                        PasswordHash = user.PasswordHash ?? string.Empty
                    };
                    await _empSvc.CreateAsync(emp, vm.Password);
                    break;

                case "Admin":
                    var adm = new DomainAdmin
                    {
                        FirstName    = vm.FirstName,
                        LastName     = vm.LastName,
                        Email        = vm.Email,
                        CityId       = vm.CityId,
                        Status       = AccountStatus.Active,
                        PhoneNumber  = vm.PhoneNumber ?? "",
                        PasswordHash = user.PasswordHash ?? string.Empty
                    };
                    await _admSvc.CreateAsync(adm, vm.Password);
                    break;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Users/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        [Area("Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            // 1) ASP.NET Identity kaydını bul
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            // 2) Domain kaydını email’e göre sil
            if (roles.Contains("Customer"))
            {
                var cust = await _custSvc.GetByEmailAsync(user.Email ?? string.Empty);
                if (cust != null)
                    await _custSvc.DeleteAsync(cust.Id);
            }
            else if (roles.Contains("Employee"))
            {
                var emp = await _empSvc.GetByEmailAsync(user.Email?? string.Empty);
                if (emp != null)
                    await _empSvc.DeleteAsync(emp.Id);
            }
            else if (roles.Contains("Admin"))
            {
                var adm = await _admSvc.GetByEmailAsync(user.Email?? string.Empty);
                if (adm != null)
                    await _admSvc.DeleteAsync(adm.Id);
            }

            // 3) Son olarak AspNetUsers tablosundaki kullanıcıyı sil
            await _accountSvc.DeleteUserAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
