// Models/Pricing.cs
namespace CarService.Web.Models;
public class Pricing
{
    public int Id { get; set; }

    // Hangi servis?
    public int ServiceTypeId { get; set; }
    public ServiceType? ServiceType { get; set; }

    public decimal Price { get; set; }

    // İsterseniz geçerlilik tarihi de ekleyebilirsiniz:
    // public DateTime? ValidFrom { get; set; }
    // public DateTime? ValidTo   { get; set; }
}
