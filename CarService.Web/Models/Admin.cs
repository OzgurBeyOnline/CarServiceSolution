// Models/Admin.cs
namespace CarService.Web.Models;
public class Admin
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int CityId { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public string PhoneNumber  { get; set; } = "";
    public City? City { get; set; }
}
