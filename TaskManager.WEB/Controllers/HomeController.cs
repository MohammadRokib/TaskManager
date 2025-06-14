using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
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
        private readonly ITaskExportService _taskExportService;

        public HomeController(
            ITaskService taskService,
            IClientService clientService,
            UserManager<User> userManager,
            ILogger<HomeController> logger,
            ITaskExportService taskExportService)
        {
            _logger = logger;
            _taskService = taskService;
            _userManager = userManager;
            _clientService = clientService;
            _taskExportService = taskExportService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            User? CurrentUser = await _userManager.GetUserAsync(User);
            var (tasks, error) = await _taskService.GetDashBoardAsync(CurrentUser!, page, pageSize);

            if (error is not null)
                return BadRequest("Error loading tasks.");

            ViewBag.Clients = await _clientService.GetClientsWithIdAsync();
            ViewBag.UserName = CurrentUser?.Name ?? "Unknown";
            ViewBag.UserId = CurrentUser?.Id ?? null;
            ViewBag.Pagesize = pageSize;

            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> LoadTasks(int page = 1, int pageSize = 10)
        {
            User? currentUser = await _userManager.GetUserAsync(User);
            var (tasks, error) = await _taskService.GetDashBoardAsync(currentUser!, page, pageSize);

            if (error is not null)
                return BadRequest("Error loading tasks.");

            return PartialView("_TaskTablePartial", tasks ?? new PaginatedTaskDashboard());
        }

        [HttpGet]
        public async Task<IActionResult> SearchParentTasks(string searchTerm = "", int take = 10)
        {
            try
            {
                var parentTasks = await _taskService.GetParentTasksAsync(searchTerm, take);
                return Json(parentTasks ?? new List<ParentTaskListViewModel>());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error searching parent tasks");
                return Json(new List<ParentTaskListViewModel>());
            }
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

        [HttpGet]
        public async Task<IActionResult> DownloadTaskExcel(CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser is null)
                    return Unauthorized("User not authenticated");

                var userId = currentUser.Id;
                //string userId = null;
                _logger.LogInformation($"Starting Excel export for user: {userId}");

                var filePath = await _taskExportService.ExportTasksToExcelSpAsync(userId, cancellationToken);
                if (!System.IO.File.Exists(filePath))
                    return NotFound("Export file could not be generated");

                //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileName = Path.GetFileName(filePath);

                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(4), CancellationToken.None);
                    await _taskExportService.CleanupExportFileAsync(filePath);
                });

                return PhysicalFile(
                    filePath,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName,
                    enableRangeProcessing: true
                );
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Excel export canceled for user");
                return StatusCode(499, "Request canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during excel export");
                return StatusCode(500, "An error occured while generating the file");
            }
        }

        public async Task<IActionResult> Edit(int? taskId)
        {
            if (taskId is null || taskId == 0)
                return NotFound();

            User? CurrentUser = await _userManager.GetUserAsync(User);
            (UpdateTaskViewModel? taskObj, string? error) = await _taskService.GetTaskByIdAsync(taskId, CurrentUser!);
            
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
