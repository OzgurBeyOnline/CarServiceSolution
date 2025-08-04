using CarService.Web.Models;
using Microsoft.AspNetCore.Identity;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;

namespace CarService.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly ICustomerService _customerService;
        private readonly ICityService _cityService;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(
            IAccountService accountService,
            IEmailService emailService,
            ICustomerService customerService,
            ICityService cityService,
            UserManager<IdentityUser> userManager,
            IServiceTypeService serviceTypeService)
        {
            _accountService = accountService;
            _emailService = emailService;
            _customerService = customerService;
            _cityService = cityService;
            _serviceTypeService = serviceTypeService;
            _userManager = userManager;
        }

        // ---- AUTH PAGE (Register + Login) ----

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Auth(string? returnUrl = null)
        {
            // returnUrl'u ViewData'ya alıyoruz
            ViewData["ReturnUrl"] = returnUrl;

            // Register form’u için şehirleri hazırla
            var vm = new RegisterViewModel
            {
                Cities = (await _cityService.GetAllAsync())
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            };
            return View(vm);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                vm.Cities = (await _cityService.GetAllAsync())
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
                ViewData["ReturnUrl"] = returnUrl;
                return View("Auth", vm);
            }

            var identityResult = await _accountService.RegisterAsync(vm.Email, vm.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var e in identityResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.Cities = (await _cityService.GetAllAsync())
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
                ViewData["ReturnUrl"] = returnUrl;
                return View("Auth", vm);
            }

            // AspNetUsers tablosuna ekleme başarılı, şimdi Customers tablosuna detay kaydet
            var customer = new Customer
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                CityId = vm.CityId
            };
            await _customerService.CreateAsync(customer, vm.Password);

            // Kayıt başarılı => eğer admin paneline yönlendirme isteniyorsa oraya, yoksa Home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                // validation hatası => Auth view’ına dön
                var regVm = new RegisterViewModel
                {
                    Cities = (await _cityService.GetAllAsync())
                        .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                };
                ViewData["ReturnUrl"] = returnUrl;
                return View("Auth", regVm);
            }

            var signInResult = await _accountService.PasswordSignInAsync(
                vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "E-posta veya parola hatalı");

                var regVm = new RegisterViewModel
                {
                    Cities = (await _cityService.GetAllAsync())
                        .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                };
                ViewData["ReturnUrl"] = returnUrl;
                return View("Auth", regVm);
            }

            // Giriş başarılıysa önce returnUrl'e yönlendir
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Yoksa Admin isen Dashboard, Employee isen randevu listesi, değilse Home
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (User.IsInRole("Employee"))
                return RedirectToAction("AllAppointments", "Appointments");

            return RedirectToAction("Index", "Home");
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ---- FORGOT PASSWORD ----

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword()
            => View(new ForgotPasswordViewModel());

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 1) Token üret
            var rawToken = await _accountService.GeneratePasswordResetTokenAsync(vm.Email);
            if (rawToken != null)
            {
                // 2) URL güvenli hale getir
                var tokenBytes = Encoding.UTF8.GetBytes(rawToken);
                var code = WebEncoders.Base64UrlEncode(tokenBytes);

                // 3) Reset link oluştur
                var callbackUrl = Url.Action(
                    "ResetPassword", "Account",
                    new { token = code, email = vm.Email },
                    protocol: Request.Scheme);

                var body = $@"
                    <p>Parolanızı yenilemek için lütfen aşağıdaki linke tıklayın:</p>
                    <p><a href=""{callbackUrl}"">Parolamı Sıfırla</a></p>
                    <p>Link geçici olarak geçerli olacaktır.</p>";

                await _emailService.SendAsync(vm.Email, "CarService - Parola Sıfırlama", body);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
            => View();

        // ---- RESET PASSWORD ----

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest("Geçersiz istek.");

            var vm = new ResetPasswordViewModel {
                Token = token,
                Email = email
            };
            return View(vm);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // token'ı orijinale döndür
            var tokenBytes = WebEncoders.Base64UrlDecode(vm.Token);
            var rawToken   = Encoding.UTF8.GetString(tokenBytes);

            var result = await _accountService.ResetPasswordAsync(
                vm.Email, rawToken, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
            => View();
    }
}
