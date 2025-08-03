using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CarService.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> ChangePasswordAsync(
            string email,
            string currentPassword,
            string newPassword)
        {
            // 1) Kullanıcıyı al
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError {
                    Code = "UserNotFound",
                    Description = "Kullanıcı bulunamadı."
                });
            }

            // 2) Mevcut şifre doğru mu?
            var isValid = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (!isValid)
            {
                return IdentityResult.Failed(new IdentityError {
                    Code = "InvalidPassword",
                    Description = "Mevcut şifre hatalı."
                });
            }

            // 3) Şifre değiştir
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result;
        }

        public async Task<IdentityResult> RegisterAsync(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<SignInResult> PasswordSignInAsync(
            string email,
            string password,
            bool isPersistent,
            bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(
                email, password, isPersistent, lockoutOnFailure);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users.ToList());
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Geçersiz kullanıcı." });

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
        public string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var rnd = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        }
    }
}
