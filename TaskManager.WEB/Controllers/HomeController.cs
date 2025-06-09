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
        private readonly IClientService _clientService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ITaskService taskService, UserManager<User> userManager, IClientService clientService)
        {
            _logger = logger;
            _taskService = taskService;
            _userManager = userManager;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            (List<TaskDashboardViewModel>? tasks, string? error) = await _taskService.GetDashBoardAsync();
            User? CurrentUser = await _userManager.GetUserAsync(User);

            ViewBag.Clients = await _clientService.GetClientsWithIdAsync();
            ViewBag.ParentTasks = await _taskService.GetParentTasksAsync();
            ViewBag.UserName = CurrentUser?.Name ?? "Unknown";
            ViewBag.UserId = CurrentUser?.Id ?? null;

            if (error is null)
                return View(tasks ?? new List<TaskDashboardViewModel>());

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

            User? CurrentUser = await _userManager.GetUserAsync(User);
            (UpdateTaskViewModel? taskObj, string? error) = await _taskService.GetTaskByIdAsync(taskId);
            
            ViewBag.Clients = await _clientService.GetClientsWithIdAsync();
            ViewBag.ParentTasks = await _taskService.GetParentTasksAsync();
            ViewBag.UserName = CurrentUser?.Name ?? "Unknown";
            ViewBag.UserId = CurrentUser?.Id ?? null;

            if (error is null)
                return View(taskObj);

            ModelState.AddModelError("", error);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTaskViewModel taskObj)
        {
            if (ModelState.IsValid)
            {
                string? error = await _taskService.UpdateTaskAsync(taskObj);
                if (error is null)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", error);
            }
            return View(taskObj);
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
