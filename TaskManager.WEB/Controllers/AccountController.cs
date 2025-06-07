using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services.IServices;

namespace TaskManager.WEB.Controllers
{
    public class AccountController(IAuthService authService) : Controller
    {
        public IActionResult Login()
        {
            if (User is not null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel request)
        {
            if (ModelState.IsValid)
            {
                string? error = await authService.LoginAsync(request);
                if (error is null)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", error);
            }
            return View(request);
        }

        public IActionResult Register()
        {
            if (User is not null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel request)
        {
            if (ModelState.IsValid)
            {
                (User? newUser, string? error) = await authService.RegisterAsync(request);
                if (error is null)
                    return RedirectToAction("Login", "Account");
                
                ModelState.AddModelError("", error);
            }
            return View(request);
        }

        public IActionResult VerifyEmail ()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            string? error = await authService.LogoutAsync();
            if (error is not null)
                ModelState.AddModelError("", error);

            return RedirectToAction("Index", "Home");
        }
    }
}
