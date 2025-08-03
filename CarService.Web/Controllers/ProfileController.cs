using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CarService.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService  _accountService;
        private readonly UserManager<IdentityUser>  _userManager;    
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmployeeService _employeeService;
        private readonly IAdminService    _adminService;
        private readonly IPasswordHashingService _passwordHasher;
        private readonly ICityService _cityService;

        public ProfileController(
            ICustomerService customerService,
            IEmployeeService employeeService,
            IAdminService adminService,
            ICityService cityService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IAccountService accountService,
            IPasswordHashingService passwordHasher)
        {
            _customerService = customerService;
            _employeeService = employeeService;
            _adminService = adminService;
            _cityService = cityService;
            _accountService = accountService;
            _passwordHasher = passwordHasher;
            _userManager     = userManager;
            _signInManager   = signInManager;
        }

        // --- MÜŞTERİ PROFİLİ (Bilgilerim) ---
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Bilgilerim()
        {
            var email = User.Identity!.Name!;
            var cust  = await _customerService.GetByEmailAsync(email);
            if (cust == null) return Forbid();

            var cities = (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();

            var vm = new CustomerEditViewModel
            {
                Id                = cust.Id,
                FirstName         = cust.FirstName,
                LastName          = cust.LastName,
                Email             = cust.Email,
                PhoneNumber       = cust.PhoneNumber,
                CityId            = cust.CityId,
                Cities            = cities,
                // ViewModel’de CurrentPassword, NewPassword, ConfirmNewPassword olmalı
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Bilgilerim(CustomerEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 1) Mevcut Identity oturumlu kullanıcı
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            return RedirectToAction("Index", "Home");
        }


        // --- ÇALIŞAN PROFİL GÜNCELLEME ---
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> EditEmployee()
        {
            var email = User.Identity!.Name!;
            var emp   = await _employeeService.GetByEmailAsync(email);
            if (emp == null) return Forbid();

            var cities = (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

            var vm = new EmployeeEditViewModel
            {
                Id          = emp.Id,
                FirstName   = emp.FirstName,
                LastName    = emp.LastName,
                Email       = emp.Email,
                PhoneNumber = emp.PhoneNumber,
                CityId      = emp.CityId,
                Cities      = cities
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> EditEmployee(EmployeeEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Cities = (await _cityService.GetAllAsync())
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
                return View(vm);
            }

            var emp = await _employeeService.GetByIdAsync(vm.Id);
            if (emp == null) return NotFound();

            emp.FirstName   = vm.FirstName;
            emp.LastName    = vm.LastName;
            emp.Email       = vm.Email;
            emp.PhoneNumber = vm.PhoneNumber;
            emp.CityId      = vm.CityId;

            await _employeeService.UpdateAsync(emp);
            return RedirectToAction("Index", "Home");
        }

        // --- ADMIN PROFİL GÜNCELLEME ---
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAdmin()
        {
            var email = User.Identity!.Name!;
            var adm   = await _adminService.GetByEmailAsync(email);
            if (adm == null) return Forbid();

            var cities = (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

            var vm = new AdminEditViewModel
            {
                Id        = adm.Id,
                FirstName = adm.FirstName,
                LastName  = adm.LastName,
                Email     = adm.Email,
                CityId    = adm.CityId,
                Cities    = cities
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAdmin(AdminEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Cities = (await _cityService.GetAllAsync())
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
                return View(vm);
            }

            var adm = await _adminService.GetByIdAsync(vm.Id);
            if (adm == null) return NotFound();

            adm.FirstName = vm.FirstName;
            adm.LastName  = vm.LastName;
            adm.Email     = vm.Email;
            adm.CityId    = vm.CityId;

            await _adminService.UpdateAsync(adm);
            return RedirectToAction("Index", "Home");
        }
    }
}
