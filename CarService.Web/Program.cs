using CarService.Web.Data;
using CarService.Web.Models;
using CarService.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Identity (User & Role yönetimi)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.AllowedForNewUsers      = true;
        options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// LoginPath artık Auth.cshtml’e gitsin
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.Cookie.HttpOnly     = true;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    opts.ExpireTimeSpan      = TimeSpan.FromHours(1);
    opts.SlidingExpiration   = true;

    opts.LoginPath        = "/Account/Auth";
    opts.AccessDeniedPath = "/Account/AccessDenied";
});

// 3) Uygulama servisleri
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IServiceTypeService, ServiceTypeService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IPageContentService, PageContentService>();

builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();
builder.Services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
builder.Services.AddScoped<IPasswordHasher<Admin>,    PasswordHasher<Admin>>();

// 4) MVC & Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// (Opsiyonel) DataSeeder
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}

// 5) HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
  .WithStaticAssets();
app.MapRazorPages();

// 6) Roller ve kullanıcı seeding (DB’den dinamik okuma)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // 1) Roller
    string[] roles = new[] { "Admin", "Employee", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // 2) Admin tablosundaki tüm kayıtları çek ve AspNetUsers’a taşı
    var allAdmins = await db.Admins.ToListAsync();
    foreach (var adm in allAdmins)
    {
        var user = await userManager.FindByEmailAsync(adm.Email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName            = adm.Email,
                NormalizedUserName  = adm.Email.ToUpperInvariant(),
                Email               = adm.Email,
                NormalizedEmail     = adm.Email.ToUpperInvariant(),
                EmailConfirmed      = true,
                PasswordHash        = adm.PasswordHash,
                SecurityStamp       = Guid.NewGuid().ToString(),
                ConcurrencyStamp    = Guid.NewGuid().ToString()
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        if (!await userManager.IsInRoleAsync(user, "Admin"))
            await userManager.AddToRoleAsync(user, "Admin");
    }

    // 3) Employee tablosundaki tüm kayıtları çek ve AspNetUsers’a taşı
    var allEmployees = await db.Employees.ToListAsync();
    foreach (var emp in allEmployees)
    {
        var user = await userManager.FindByEmailAsync(emp.Email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName            = emp.Email,
                NormalizedUserName  = emp.Email.ToUpperInvariant(),
                Email               = emp.Email,
                NormalizedEmail     = emp.Email.ToUpperInvariant(),
                EmailConfirmed      = true,
                PasswordHash        = emp.PasswordHash,
                SecurityStamp       = Guid.NewGuid().ToString(),
                ConcurrencyStamp    = Guid.NewGuid().ToString()
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        if (!await userManager.IsInRoleAsync(user, "Employee"))
            await userManager.AddToRoleAsync(user, "Employee");
    }

    // 4) Customer rol ataması (kayıt formu zaten kullanıcı yaratıyor)
    var custEmails = await db.Customers
                             .Select(c => c.Email)
                             .Distinct()
                             .ToListAsync();
    foreach (var email in custEmails)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null && !await userManager.IsInRoleAsync(user, "Customer"))
            await userManager.AddToRoleAsync(user, "Customer");
    }
}

app.Run();
