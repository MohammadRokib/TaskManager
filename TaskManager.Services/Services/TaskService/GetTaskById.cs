using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.Services.TaskService
{
    public partial class TaskService
    {
        public async Task<(UpdateTaskViewModel?, string?)> GetTaskByIdAsync(int? taskId, User request)
        {
            try
            {
                UpdateTaskViewModel? taskObj = await _context.Tasks
                .Where(t => t.TaskId == taskId && t.UserId == request.Id)
                .Include(t => t.Client)
                .Select(t => new UpdateTaskViewModel()
                {
                    TaskId = t.TaskId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    Severity = t.Severity,
                    IsParent = t.IsParent,
                    ClientId = t.ClientId,
                    Client = t.Client,
                    UserId = t.UserId,
                    ParentTaskId = t.ParentTaskId
                })
                .FirstOrDefaultAsync();

                if (taskId is not null && taskId != 0)
                {
                    taskObj!.ParentTask = await GetParentTask(taskObj!.ParentTaskId ?? 0);
                }

                if (taskObj is null)
                    return (null, "Task not found");

                return (taskObj, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (null, "Something went wrong");
            }
        }

        private async Task<ParentTaskListViewModel?> GetParentTask(int? id)
        {
            if (id is null || id == 0)
                return new ParentTaskListViewModel();

            ParentTaskListViewModel? parentTask = await _context.Tasks
                .Where(t => t.TaskId == id)
                .Select(t => new ParentTaskListViewModel()
                {
                    Value = t.TaskId,
                    Text = t.Title
                })
                .FirstOrDefaultAsync();

            return parentTask;
        }
    }
}
