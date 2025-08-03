// Models/Customer.cs
namespace CarService.Web.Models;
public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Şehire bağlama
    public int CityId { get; set; }
    public City? City { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    // Randevuları
    public ICollection<Appointment>? Appointments { get; set; }
}
