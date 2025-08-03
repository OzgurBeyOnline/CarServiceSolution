using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagementController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /UserManagement/LockedOut
        public IActionResult LockedOut()
        {
            // LockoutEnd > UtcNow olanlar kilitli
            var now = DateTimeOffset.UtcNow;
            var locked = _userManager.Users
                .Where(u => u.LockoutEnd != null && u.LockoutEnd > now)
                .Select(u => new LockedOutUserViewModel {
                    Id          = u.Id,
                    Email       = u.Email,
                    LockoutEnd  = u.LockoutEnd!.Value
                })
                .ToList();
            return View(locked);
        }

        // POST: /UserManagement/Unlock
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Kilidi kaldır
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                await _userManager.ResetAccessFailedCountAsync(user);
            }
            return RedirectToAction(nameof(LockedOut));
        }
    }

    // Sadece ViewModel
    public class LockedOutUserViewModel
    {
        public string Id { get; set; } = "";
        public string? Email { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
    }
}
