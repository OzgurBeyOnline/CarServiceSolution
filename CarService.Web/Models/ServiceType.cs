// Models/ServiceType.cs
namespace CarService.Web.Models;
public class ServiceType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Pricing> Pricings { get; set; }  = new List<Pricing>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
