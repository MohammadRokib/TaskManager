using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.WEB.Controllers
{
    public class ProfileController(UserManager<User> userManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        public async Task<IActionResult> Index()
        {
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
    }
}
