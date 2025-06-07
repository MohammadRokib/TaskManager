using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.DataAccess.Data;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services;
using TaskManager.Services.IServices;
using TaskManager.WEB.Models;

namespace TaskManager.WEB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ITaskService taskService, UserManager<User> userManager)
        {
            _logger = logger;
            _taskService = taskService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            (List<TaskDashboardViewModel>? tasks,
                List<ClientListViewModel>? clients,
                List<ParentTaskListViewModel>? parentTasks,
                string? error) = await _taskService.GetDashBoardAsync();

            ViewBag.Clients = clients;
            ViewBag.ParentTasks = parentTasks;

            User? CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser is null)
            {
                ViewBag.ErrorMessage = "User not found. Please log in.";
                ViewBag.UserName = "Unknown";
                ViewBag.UserId = null;
            }
            else
            {
                ViewBag.UserName = CurrentUser.Name;
                ViewBag.UserId = CurrentUser.Id;
            }

            if (error is null)
            {
                return View(tasks ?? new List<TaskDashboardViewModel>());
            }

            ModelState.AddModelError("", error!);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(AddTaskViewModel request)
        {
            Console.WriteLine(request);
            if (ModelState.IsValid)
            {
                string? error = await _taskService.AddTaskAsync(request);
                if (error is null)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", error);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(int? taskId)
        {
            if (taskId is null || taskId == 0)
                return NotFound();

            return View(new AddTaskViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
