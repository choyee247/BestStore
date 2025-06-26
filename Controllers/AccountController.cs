using Example2.Data;
using Example2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Example2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext context;

        public AccountController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                context.Users.Add(user);
                context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = context.Users
                .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                // Clear existing session
                HttpContext.Session.Clear();

                // Set new session values
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                HttpContext.Session.SetString("Email", existingUser.Email);
                HttpContext.Session.SetString("Role", existingUser.Role ?? "User");

                // Debug output
                Console.WriteLine($"User logged in with role: {existingUser.Role}");

                // Redirect based on role
                return existingUser.Role == "Admin"
                    ? RedirectToAction("Index", "Admin")
                    : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(user);
        }

    }
}

