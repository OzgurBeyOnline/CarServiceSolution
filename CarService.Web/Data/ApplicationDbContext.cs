using CarService.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarService.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City>         Cities        { get; set; }
        public DbSet<Customer>     Customers     { get; set; }
        public DbSet<Employee>     Employees     { get; set; }
        public DbSet<Admin>        Admins        { get; set; }
        public DbSet<ServiceType>  ServiceTypes  { get; set; }
        public DbSet<Pricing>      Pricings      { get; set; }
        public DbSet<Appointment>  Appointments  { get; set; }
        public DbSet<PageContent> PageContents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tablo isimleri
            builder.Entity<City>().ToTable("Cities");
            builder.Entity<Customer>().ToTable("Customers");
            builder.Entity<Employee>().ToTable("Employees");
            builder.Entity<Admin>().ToTable("Admins");
            builder.Entity<ServiceType>().ToTable("ServiceTypes");
            builder.Entity<Pricing>().ToTable("Pricings");
            builder.Entity<Appointment>().ToTable("Appointments");

            // Decimal precision
            builder.Entity<Pricing>()
                   .Property(p => p.Price)
                   .HasPrecision(18, 2);

            builder.Entity<Appointment>()
                   .Property(a => a.Price)
                   .HasPrecision(18, 2);

            // Appointments ↔ Customer  : Cascade (bir müşteri silinirse randevuları da silinsin)
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointments ↔ Employee  : Restrict (bir çalışan silinemez, önce randevular silinmeli)
            builder.Entity<Appointment>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Appointments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointments ↔ ServiceType : Restrict (bir servis tipi silinemez, önce randevular silinmeli)
            builder.Entity<Appointment>()
                .HasOne(a => a.ServiceType)
                .WithMany(st => st.Appointments)
                .HasForeignKey(a => a.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
