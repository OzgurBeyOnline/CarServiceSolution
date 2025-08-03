namespace CarService.Web.Services
{
    public interface IPasswordHashingService
    {
        
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
