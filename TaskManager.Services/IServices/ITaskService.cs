using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.IServices
{
    public interface ITaskService
    {
        Task<string?> AddTaskAsync(AddTaskViewModel request);
        Task<string?> UpdateTaskAsync(UpdateTaskViewModel taskObj);
        Task<List<ParentTaskListViewModel>?> GetParentTasksAsync();
        Task<(List<TaskDashboardViewModel>?, string?)> GetDashBoardAsync();
        Task<(UpdateTaskViewModel?, string?)> GetTaskByIdAsync(int? taskId);
    }
}
