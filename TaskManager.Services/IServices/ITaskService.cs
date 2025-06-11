using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.IServices
{
    public interface ITaskService
    {
        Task<string?> AddTaskAsync(AddTaskViewModel request);
        Task<string?> UpdateTaskAsync(UpdateTaskViewModel taskObj);
        Task<List<ParentTaskListViewModel>?> GetParentTasksAsync(string? searchTerm = null, int take = 10);
        Task<(UpdateTaskViewModel?, string?)> GetTaskByIdAsync(int? taskId, User request);
        Task<(PaginatedTaskDashboard?, string?)> GetDashBoardAsync(User request, int pageNumber, int pageSize);
    }
}
