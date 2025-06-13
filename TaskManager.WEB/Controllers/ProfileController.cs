using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services.IServices;

namespace TaskManager.WEB.Controllers
{
    [Authorize]
    public class ProfileController(UserManager<User> userManager, IProfileService profileService) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IProfileService _profileService = profileService;

        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Running tests\n\n\n\n\n\n\n\n\n");
            try
            {
                User? currentUser = await _userManager.GetUserAsync(User);
                if (currentUser is not null)
                {
                    UserProfileViewModel userObj = new UserProfileViewModel()
                    {
                        Id = currentUser.Id,
                        Name = currentUser.Name,
                        Email = currentUser.Email,
                        Phone = currentUser.PhoneNumber,
                        Address = currentUser.Address,
                        Department = currentUser.Department,
                        Designation = currentUser.Designation
                    };
                    return View(userObj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return NotFound();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserProfileViewModel request)
        {
            if (ModelState.IsValid)
            {
                string? error = await _profileService.UpdateUserAsync(request);
                if (error is null)
                    return RedirectToAction("Index", "Profile");

                ModelState.AddModelError("", error);
            }
            
            ViewBag.IsEditMode = true;
            return View("Index", request);
        }
    }
}
