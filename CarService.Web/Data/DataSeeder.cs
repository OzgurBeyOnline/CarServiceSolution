using CarService.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Web.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var ctx = sp.GetRequiredService<ApplicationDbContext>();
            var hasher = sp.GetRequiredService<IPasswordHasher<Customer>>();

            // 1) Şehirleri ekle
            if (!ctx.Cities.Any())
            {
                var iller = new[]
                {
                    "Adana","Adıyaman","Afyonkarahisar","Ağrı","Amasya","Ankara","Antalya","Artvin","Aydın",
                    "Balıkesir","Bilecik","Bingöl","Bitlis","Bolu","Burdur","Bursa","Çanakkale","Çankırı","Çorum",
                    "Denizli","Diyarbakır","Edirne","Elazığ","Erzincan","Erzurum","Eskişehir","Gaziantep","Giresun",
                    "Gümüşhane","Hakkâri","Hatay","Isparta","Mersin","İstanbul","İzmir","Kars","Kastamonu","Kayseri",
                    "Kırıkkale","Kırklareli","Kırşehir","Kocaeli","Konya","Kütahya","Malatya","Manisa","Kahramanmaraş",
                    "Mardin","Muğla","Muş","Nevşehir","Niğde","Ordu","Rize","Sakarya","Samsun","Siirt","Sinop","Sivas",
                    "Tekirdağ","Tokat","Trabzon","Tunceli","Şanlıurfa","Uşak","Van","Yozgat","Zonguldak","Aksaray",
                    "Bayburt","Karaman","Kırıkkale","Batman","Şırnak","Bartın","Ardahan","Iğdır","Yalova","Karabük",
                    "Kilis","Osmaniye","Düzce"
                };
                var cities = iller
                    .Select((name, i) => new City { Id = i + 1, Name = name })
                    .ToList();
                ctx.Cities.AddRange(cities);
                await ctx.SaveChangesAsync();
            }

            // 2) Admin kullanıcı ekle (PasswordHasher<Admin> kullanıyorsanız ona göre değiştirin)
            if (!ctx.Admins.Any())
            {
                var admin = new Admin
                {
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    Email = "admin@carservice.com",
                    CityId = 34,             // örneğin İstanbul
                    Status = AccountStatus.Active
                };
                // Parolayı hash’le
                var adminHasher = sp.GetRequiredService<IPasswordHasher<Admin>>();
                admin.PasswordHash = adminHasher.HashPassword(admin, "Admin@123");
                ctx.Admins.Add(admin);
                await ctx.SaveChangesAsync();
            }

            // 3) 4 çalışan ekle
            if (!ctx.Employees.Any())
            {
                var employees = new[]
                {
                    new Employee { FirstName="Ayşe", LastName="Yılmaz", Email="ayse@carservice.com", PhoneNumber="05001234567", CityId=34, Status=AccountStatus.Active},
                    new Employee { FirstName="Mehmet", LastName="Demir", Email="mehmet@carservice.com", PhoneNumber="05007654321", CityId=6, Status=AccountStatus.Active},
                    new Employee { FirstName="Elif", LastName="Kara", Email="elif@carservice.com", PhoneNumber="05009876543", CityId=35, Status=AccountStatus.Active},
                    new Employee { FirstName="Burak", LastName="Şahin", Email="burak@carservice.com", PhoneNumber="05005432198", CityId=1, Status=AccountStatus.Active}
                }.ToList();
                var empHasher = sp.GetRequiredService<IPasswordHasher<Employee>>();
                foreach (var e in employees)
                    e.PasswordHash = empHasher.HashPassword(e, "Employee@123");
                ctx.Employees.AddRange(employees);
                await ctx.SaveChangesAsync();
            }

            // 4) 5 müşteri ekle
            if (!ctx.Customers.Any())
            {
                var customers = new[]
                {
                    new Customer { FirstName="Ali", LastName="Veli", Email="ali@carservice.com", PhoneNumber="05001112233", CityId=34, Status=AccountStatus.Active},
                    new Customer { FirstName="Ayşe", LastName="Fatma", Email="aysef@carservice.com", PhoneNumber="05004455667", CityId=6, Status=AccountStatus.Active},
                    new Customer { FirstName="Can", LastName="Polat", Email="canp@carservice.com", PhoneNumber="05007778899", CityId=1, Status=AccountStatus.Active},
                    new Customer { FirstName="Deniz", LastName="Kaya", Email="deniz@carservice.com", PhoneNumber="05009990011", CityId=35, Status=AccountStatus.Active},
                    new Customer { FirstName="Ece", LastName="Çelik", Email="ece@carservice.com", PhoneNumber="05003332211", CityId=16, Status=AccountStatus.Active}
                }.ToList();
                foreach (var c in customers)
                    c.PasswordHash = hasher.HashPassword(c, "Customer@123");
                ctx.Customers.AddRange(customers);
                await ctx.SaveChangesAsync();
            }

            // 5) Örnek ServiceType ve Pricing ekleyebilirsiniz benzer şekilde…
            if (!ctx.ServiceTypes.Any())
            {
                // 5a) ServiceType’ları ekle
                var serviceTypes = new[]
                {
                    new ServiceType
                    {
                        Name = "Yağ Değişimi",
                        Description = "Motor yağınızın ve filtresinin değiştirilmesi."
                    },
                    new ServiceType
                    {
                        Name = "Lastik Değişimi",
                        Description = "Tüm lastiklerin söküm, takım ve balans ayarı."
                    },
                    new ServiceType
                    {
                        Name = "Fren Bakımı",
                        Description = "Fren balata ve disk kontrolleri ile gerekli parçaların değişimi."
                    },
                    new ServiceType
                    {
                        Name = "Akü Kontrolü",
                        Description = "Akü testi ve gerekiyorsa akü değişimi."
                    },
                    new ServiceType
                    {
                        Name = "Motor Diagnostiği",
                        Description = "Arıza kodu okuma ve detaylı teşhis."
                    }
                }.ToList();

                ctx.ServiceTypes.AddRange(serviceTypes);
                await ctx.SaveChangesAsync();

                // 5b) Her ServiceType için Pricing ekle
                var pricings = new List<Pricing>
                {
                    new Pricing { ServiceTypeId = serviceTypes[0].Id, Price = 299.99m },
                    new Pricing { ServiceTypeId = serviceTypes[1].Id, Price = 499.50m },
                    new Pricing { ServiceTypeId = serviceTypes[2].Id, Price = 399.00m },
                    new Pricing { ServiceTypeId = serviceTypes[3].Id, Price = 199.75m },
                    new Pricing { ServiceTypeId = serviceTypes[4].Id, Price = 549.25m }
                };

                ctx.Pricings.AddRange(pricings);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
