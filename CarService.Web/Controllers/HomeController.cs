using CarService.Web.Data;    
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;


namespace CarService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController>    _logger;
        private readonly ICityService               _cityService;
        private readonly IServiceTypeService        _serviceTypeService;
        private readonly IAppointmentService        _appointmentService;
        private readonly ICustomerService           _customerService;
        private readonly IConfiguration             _configuration;
        private readonly IJsonContentService        _jsonSvc;
        private readonly ApplicationDbContext       _context;
        

        public HomeController(
            ILogger<HomeController> logger,
            ICityService cityService,
            IJsonContentService jsonSvc,
            IServiceTypeService serviceTypeService,
            IAppointmentService appointmentService,
            ICustomerService customerService,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _cityService = cityService;
            _jsonSvc = jsonSvc;
            _serviceTypeService = serviceTypeService;
            _appointmentService = appointmentService;
            _customerService = customerService;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Sections = await _jsonSvc.LoadAsync();
            // 1) Register formu için şehir listesini hazırla
            var vm = new RegisterViewModel
            {
                Cities = (await _cityService.GetAllAsync())
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            };

            // 2) Hizmetlerimiz bölümü için servis tiplerini çek ve ViewBag'e at
            var services = await _serviceTypeService.GetAllAsync();
            ViewBag.ServiceTypes = services;

            // 3) Randevu panelini sadece giriş yapmış kullanıcılara göster
            if (User.Identity?.IsAuthenticated == true)
            {
                ViewBag.ApptVm = new AppointmentCreateViewModel
                {
                    ServiceTypes = services
                        .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                        .ToList()
                };
            }
            else
            {
                ViewBag.ApptVm = null;
            }            

            return View(vm);
        }

        public async Task<IEnumerable<ServiceType>> GetAllAsync()
        {
            return await _context.ServiceTypes
                .Include(st => st.Pricings)
                .AsNoTracking()
                .ToListAsync();
        }


        [HttpGet]
        public async Task<IActionResult> About()
        {
            // JSON'u oku
            Dictionary<string, string> model = await _jsonSvc.LoadAsync();
            // View'a gönder
            return View(model);
        }

        // ---- İLETİŞİM ----
        [HttpGet]
        public IActionResult Contact() => View(new ContactViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var smtp        = _configuration.GetSection("Smtp");
            var host        = smtp.GetValue<string>("Host")!;
            var port        = smtp.GetValue<int>("Port");
            var enableSsl   = smtp.GetValue<bool>("EnableSsl");
            var user        = smtp.GetValue<string>("User")!;
            var pass        = smtp.GetValue<string>("Pass")!;
            var toAddress   = smtp.GetValue<string>("From")!;
            var displayName = smtp.GetValue<string>("DisplayName")!;

            var mail = new MailMessage
            {
                From       = new MailAddress(vm.Email, $"{vm.FirstName} {vm.LastName}"),
                Subject    = $"[İletişim Formu] {vm.FirstName} {vm.LastName}",
                IsBodyHtml = true,
                Body       = $@"
                    <p><strong>İsim:</strong> {vm.FirstName}</p>
                    <p><strong>Soyisim:</strong> {vm.LastName}</p>
                    <p><strong>Email:</strong> {vm.Email}</p>
                    <p><strong>Telefon:</strong> {vm.PhoneNumber ?? "-"} </p>
                    <p><strong>Mesaj:</strong><br/>{vm.Message}</p>"
            };
            mail.To.Add(toAddress);
            mail.ReplyToList.Add(new MailAddress(vm.Email, $"{vm.FirstName} {vm.LastName}"));

            using var client = new SmtpClient(host, port)
            {
                EnableSsl   = enableSsl,
                Credentials = new NetworkCredential(user, pass)
            };
            await client.SendMailAsync(mail);

            TempData["ContactSuccess"] = "Mesajınız başarıyla iletildi. Teşekkür ederiz!";
            return RedirectToAction(nameof(Contact));
        }

        [HttpGet]
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
