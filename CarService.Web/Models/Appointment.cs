// Models/Appointment.cs
namespace CarService.Web.Models;

public class Appointment
{
    public int Id { get; set; }

    // Kim randevu aldı?
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Kiminle?
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    // Hangi servis?
    public int ServiceTypeId { get; set; }
    public ServiceType? ServiceType { get; set; }

    // Fiyat, randevu tarihi, durum...
    public decimal Price { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;  // Örn: Beklemede, Onaylandı, Gerçekleşti, İptal
    public string Notes { get; set; } = string.Empty;
}
