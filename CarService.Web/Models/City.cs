// Models/City.cs
namespace CarService.Web.Models;
public class City
{
    public int Id { get; set; }
    public string? Name { get; set; }

    // Navigation
    public ICollection<Customer>? Customers { get; set; }
    public ICollection<Employee>? Employees { get; set; }
    public ICollection<Admin>? Admins { get; set; }
}
