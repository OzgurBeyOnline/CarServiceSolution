using Microsoft.AspNetCore.Identity;

namespace CarService.Web.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        // object tipini kullanıyoruz, TUser bilgisi hash algoritması için şart değil
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
            => _hasher.HashPassword(null!, password);

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
            => _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword)
                       == PasswordVerificationResult.Success;
    }
}
