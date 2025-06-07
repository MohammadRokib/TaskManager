using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.ViewModels;
using TaskManager.Services;
using TaskManager.Services.IServices;

namespace TaskManager.WEB.Controllers
{
    public class TaskController(/*ITaskService taskService*/) : Controller
    {
        //public async Task<IActionResult> Index()
        //{
        //    (List<TaskDashboardViewModel>? tasks,
        //        List<ClientListViewModel>? clients,
        //        List<ParentTaskListViewModel>? parentTasks,
        //        string? error) = await taskService.GetDashBoardAsync();
        //    if (error is null)
        //        return View(tasks);
            
        //    ModelState.AddModelError("", error);
        //    return View();
        //}

        ////if (ModelState.IsValid)
        ////    {
        ////        string? error = await authService.LoginAsync(request);
        ////        if (error is null)
        ////            return RedirectToAction("Index", "Home");

        ////ModelState.AddModelError("", error);
        ////    }
        ////    return View(request);
}
}
