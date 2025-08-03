using CarService.Web.Models;
using CarService.Web.Services;
using CarService.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace CarService.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private const int PageSize = 5;

        private readonly IAppointmentService _appointmentService;
        private readonly ICustomerService    _customerService;
        private readonly IEmployeeService    _employeeService;
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IEmailService       _emailService;
        private readonly ICityService        _cityService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            ICustomerService    customerService,
            IEmployeeService    employeeService,
            IServiceTypeService serviceTypeService,
            IEmailService       emailService,
            ICityService        cityService)
        {
            _appointmentService   = appointmentService;
            _customerService      = customerService;
            _employeeService      = employeeService;
            _serviceTypeService   = serviceTypeService;
            _emailService         = emailService;
            _cityService          = cityService;
        }
        
        public async Task<IActionResult> MyAppointments(int page = 1)
        {
            var email = User.Identity!.Name!;
            var cust  = await _customerService.GetByEmailAsync(email);
            if (cust == null) return Forbid();

            var list = await _appointmentService
                .GetPagedByCustomerAsync(cust.Id, page, PageSize, statusFilter: null);
            return View("MyAppointments", list);
        }

        // LISTELEME (Paging + Filtering)
        public async Task<IActionResult> Index(int page = 1, string? statusFilter = null)
        {
            ViewData["StatusFilter"] = statusFilter;

            if (User.IsInRole("Customer"))
            {
                var email = User.Identity!.Name!;
                var cust = await _customerService.GetByEmailAsync(email);
                if (cust == null) return Forbid();

                var list = await _appointmentService
                    .GetPagedByCustomerAsync(cust.Id, page, PageSize, statusFilter);
                return View("IndexCustomer", list);
            }
            else
            {
                var list = await _appointmentService
                    .GetPagedAsync(page, PageSize, statusFilter);
                return View("IndexAdmin", list);
            }
        }
        
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> AllAppointments(string? searchName)
        {
            var all = await _appointmentService.GetAllAsync();

            // ViewModel’e dönüştür
            var vm = all.Select(a => new AppointmentListViewModel {
                Id               = a.Id,
                CustomerName = $"{a.Customer?.FirstName ?? ""} {a.Customer?.LastName ?? ""}".Trim(),
                EmployeeName = $"{a.Employee?.FirstName ?? ""} {a.Employee?.LastName ?? ""}".Trim(),
                ServiceType = $"{a.ServiceType?.Name ?? ""}",
                AppointmentDate  = a.AppointmentDate,
                Price = a.Price,
                Status = a.Status
            });

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                vm = vm.Where(x => x.CustomerName
                    .Contains(searchName, StringComparison.OrdinalIgnoreCase));
            }

            ViewData["SearchName"] = searchName;
            return View(vm);
        }

        // CREATE: GET
        public async Task<IActionResult> Create()
        {
            // Şehir listesini al
            var cities = (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();

            if (User.IsInRole("Customer"))
            {
                // Müşteri rolündeki kullanıcılar:
                var vm = new AppointmentCreateViewModel
                {
                    Cities = cities,
                    ServiceTypes = (await _serviceTypeService.GetAllAsync())
                        .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                        .ToList()
                };
                // Çalışanlar başlangıçta ya tüm çalışanlar ya da boş list
                vm.Employees = new List<SelectListItem> {
                    new SelectListItem("– Şehir seçin –", "")
                };
                return View("CreateCustomer", vm);
            }
            else
            {
                // Admin/Çalışan kullanıcılar:
                var vm = new AppointmentAdminCreateViewModel
                {
                    Cities = cities,
                    Customers = (await _customerService.GetAllAsync())
                        .Select(c => new SelectListItem($"{c.FirstName} {c.LastName}", c.Id.ToString()))
                        .ToList(),
                    Employees = (await _employeeService.GetAllAsync())
                        .Select(e => new SelectListItem($"{e.FirstName} {e.LastName}", e.Id.ToString()))
                        .ToList(),
                    ServiceTypes = (await _serviceTypeService.GetAllAsync())
                        .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                        .ToList()
                };
                return View("CreateAdmin", vm);
            }
        }

        // CREATE: POST for Customer
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(AppointmentCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("CreateCustomer", vm);

            var email = User.Identity!.Name!;
            var cust  = await _customerService.GetByEmailAsync(email);
            if (cust == null) 
                return Forbid();

            // Çoklu servis tipi için döngüye alıyoruz
            foreach (var serviceTypeId in vm.SelectedServiceTypeIds)
            {
                var svcType = await _serviceTypeService.GetByIdAsync(serviceTypeId);
                if (svcType == null)
                {
                    // Geçersiz seçim atlanabilir veya ModelState hatası eklenebilir
                    continue;
                }

                decimal price = svcType.Pricings.FirstOrDefault()?.Price ?? 0;

                var appt = new Appointment {
                    CustomerId      = cust.Id,
                    EmployeeId      = vm.EmployeeId,           // dropdown’dan gelen
                    ServiceTypeId   = serviceTypeId,           // vm.ServiceTypeId yerine
                    AppointmentDate = vm.AppointmentDate,
                    Price           = price,
                    Status          = "Active",
                    Notes           = vm.Notes ?? string.Empty // null yerine boş string
                };
                await _appointmentService.CreateAsync(appt);

                // Onay e-postası
                var employee = await _employeeService.GetByIdAsync(appt.EmployeeId);
                var body = $@"
                    <h3>Randevu Onayı</h3>
                    <p>Merhaba {cust.FirstName},</p>
                    <ul>
                    <li><strong>Tarih:</strong> {appt.AppointmentDate:g}</li>
                    <li><strong>Şehir:</strong> {cust.City?.Name}</li>
                    <li><strong>Servis:</strong> {svcType.Name}</li>
                    <li><strong>Çalışan:</strong> {employee?.FirstName} {employee?.LastName}</li>
                    <li><strong>Fiyat:</strong> {appt.Price:N2} TL</li>
                    </ul>
                    <p>Teşekkürler,<br/>CarService Ekibi</p>";
                await _emailService.SendAsync(cust.Email, "Randevu Onayınız", body);
            }

            return RedirectToAction("Index", "Home");
        }


        // CREATE: GET for Admin/Employee
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreateAdmin()
        {
            // 1) Şehir listesini al
            var cities = (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();

            // 2) Müşteri dropdown’u
            var customers = (await _customerService.GetAllAsync())
                .Select(c => new SelectListItem($"{c.FirstName} {c.LastName}", c.Id.ToString()))
                .ToList();

            // 3) Servis tipleri dropdown’u
            var svcTypes = (await _serviceTypeService.GetAllAsync())
                .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                .ToList();

            // 4) Başlangıçta şehir seçilmemiş, çalışan listesi boş placeholder’lı
            var emps = new List<SelectListItem> {
                new SelectListItem("– Şehir seçin –", "")
            };

            var vm = new AppointmentAdminCreateViewModel
            {
                Cities       = cities,
                Customers    = customers,
                ServiceTypes = svcTypes,
                Employees    = emps,
                // CityId, CustomerId, EmployeeId vb. default 0
            };
            return View("CreateAdmin", vm);
        }

        // CREATE: POST for Admin/Employee
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreateAdmin(AppointmentAdminCreateViewModel vm)
        {
            // 1) Şehir/Customer/ServiceTypes dropdown’larını her durumda yeniden doldur
            vm.Cities       = (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
            vm.Customers    = (await _customerService.GetAllAsync())
                .Select(c => new SelectListItem($"{c.FirstName} {c.LastName}", c.Id.ToString()))
                .ToList();
            vm.ServiceTypes = (await _serviceTypeService.GetAllAsync())
                .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                .ToList();

            // Çalışan listesini de, seçili CityId’ye göre doldur (veya placeholder)
            vm.Employees = (vm.CityId > 0)
                ? (await _employeeService.GetByCityAsync(vm.CityId))
                    .Select(e => new SelectListItem($"{e.FirstName} {e.LastName}", e.Id.ToString()))
                    .ToList()
                : new List<SelectListItem> { new SelectListItem("– Şehir seçin –", "") };

            if (!ModelState.IsValid)
                return View("CreateAdmin", vm);

            // 2) Çoklu servis tipi seçimini işleyelim
            foreach (var svcId in vm.SelectedServiceTypeIds)
            {
                var svcType = await _serviceTypeService.GetByIdAsync(svcId);
                if (svcType == null) 
                    continue;

                var price = svcType.Pricings.FirstOrDefault()?.Price ?? 0m;

                var appt = new Appointment
                {
                    CustomerId      = vm.CustomerId,
                    EmployeeId      = vm.EmployeeId,
                    ServiceTypeId   = svcId,
                    AppointmentDate = vm.AppointmentDate,
                    Price           = price,
                    Status          = "Beklemede",
                    Notes           = vm.Notes ?? string.Empty
                };
                await _appointmentService.CreateAsync(appt);

                // Onay e-postası
                var cust     = await _customerService.GetByIdAsync(vm.CustomerId);
                var employee = await _employeeService.GetByIdAsync(vm.EmployeeId);
                var body = $@"
                    <h3>Randevu Onayı</h3>
                    <p>Merhaba {cust?.FirstName},</p>
                    <ul>
                      <li><strong>Tarih:</strong> {appt.AppointmentDate:g}</li>
                      <li><strong>Şehir:</strong> {(cust?.City?.Name ?? "")}</li>
                      <li><strong>Servis:</strong> {svcType.Name}</li>
                      <li><strong>Çalışan:</strong> {employee?.FirstName} {employee?.LastName}</li>
                      <li><strong>Fiyat:</strong> {appt.Price:N2} TL</li>
                    </ul>
                    <p>CarService Ekibi</p>";
                await _emailService.SendAsync(cust?.Email ?? "", "Randevu Onayınız", body);
            }

            // 3) İşlem tamamlanınca Anasayfa’ya dön
            return RedirectToAction("Index", "Home");
        }

        // DELETE
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // EDIT: GET
        public async Task<IActionResult> Edit(int id)
        {
            var appt = await _appointmentService.GetByIdAsync(id);
            if (appt == null) return NotFound();

            var vm = new AppointmentEditViewModel {
                Id              = appt.Id,
                ServiceTypeId   = appt.ServiceTypeId,
                AppointmentDate = appt.AppointmentDate,
                Notes           = appt.Notes,
                ServiceTypes    = (await _serviceTypeService.GetAllAsync())
                    .Select(st => new SelectListItem(st.Name, st.Id.ToString()))
                    .ToList()
            };
            return View(vm);
        }

        // EDIT: POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var appt = await _appointmentService.GetByIdAsync(vm.Id);
            if (appt == null) return NotFound();

            appt.ServiceTypeId   = vm.ServiceTypeId;
            appt.AppointmentDate = vm.AppointmentDate;
            appt.Notes           = vm.Notes!;
            await _appointmentService.UpdateAsync(appt);

            return RedirectToAction(nameof(Index));
        }
    }
}
