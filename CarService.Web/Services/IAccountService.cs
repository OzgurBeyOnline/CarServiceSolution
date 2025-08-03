using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CarService.Web.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(string email, string password);
        Task<SignInResult> PasswordSignInAsync(
            string email,
            string password,
            bool isPersistent,
            bool lockoutOnFailure);
        Task SignOutAsync();
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task DeleteUserAsync(string userId);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
        string GenerateRandomPassword(int length = 10);
    }
}
