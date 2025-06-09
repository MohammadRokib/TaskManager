using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess.Data;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services.IServices;

namespace TaskManager.Services.Services.TaskService
{
    public partial class TaskService(UserManager<User> userManager, TaskManagerDbContext context, IClientService clientService) : ITaskService
    {
        protected readonly TaskManagerDbContext _context = context;
        protected readonly UserManager<User> _userManager = userManager;
        protected readonly IClientService _clientService = clientService;

        public async Task<List<ParentTaskListViewModel>?> GetParentTasksAsync()
        {
            try
            {
                List<ParentTaskListViewModel>? ParentTasksList = await _context.Tasks
                        .Where(t => t.IsParent)
                        .Select(t => new ParentTaskListViewModel()
                        {
                            Value = t.TaskId,
                            Text = t.Title
                        })
                        .ToListAsync();
                return ParentTasksList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<ParentTaskListViewModel>();
            }
        }
    }
}
