using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.ViewModels;
using TaskManager.Services;
using TaskManager.Services.IServices;

namespace TaskManager.WEB.Controllers
{
    [Authorize]
    public class ClientController(IClientService clientService) : Controller
    {
        private readonly IClientService _clientService = clientService;
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Running tests");
            (List<ClientDashboardViewModel>? clients, string? error) = await _clientService.GetClientDashBoardAsync();
            if (error is null)
                return View(clients);

            ModelState.AddModelError("", error);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddClientViewModel request)
        {
            if (ModelState.IsValid)
            {
                string? error = await _clientService.AddClient(request);
                if (error is null)
                    return RedirectToAction("Index", "Client");

                ModelState.AddModelError("", error);
            }
            return PartialView("_AddClientModal", request);
        }
    }
}
