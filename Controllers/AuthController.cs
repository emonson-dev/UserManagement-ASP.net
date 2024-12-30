using Microsoft.AspNetCore.Mvc;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
namespace UserManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManagementContext _context;

        public AuthController(UserManagementContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(p => p.UserName == model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User is not existed.");
                    return View(model);
                }
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Password, model.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    if (user.Status == Status.Approved)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // Sign in the user and create an authentication cookie
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        if (user.UserName == "Admin")
                        {
                            return RedirectToAction("Index", "Users");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Account is not approved");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Password is wrong!");
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(p => p.UserName == model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "User already exists.");
                    return View(model);
                }
                var newUser = new User
                {
                    UserName = model.UserName,
                    Password = HashPassword(model.Password),
                    Status = Status.Pending,
                    EnrollmentDate = DateTime.UtcNow
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Auth");

            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(null, password);
        }
    }
}
