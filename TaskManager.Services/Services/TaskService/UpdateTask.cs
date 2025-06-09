using Microsoft.EntityFrameworkCore;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.Services.TaskService
{
    public partial class TaskService
    {
        public async Task<string?> UpdateTaskAsync(UpdateTaskViewModel taskObj)
        {
            try
            {
                Models.Entities.Task? taskFromDb = await _context.Tasks
                    .Where(t => t.TaskId == taskObj.TaskId)
                    .FirstOrDefaultAsync();

                if (taskFromDb is null)
                    return "Task not found";

                taskFromDb.Title = taskObj.Title;
                taskFromDb.Description = taskObj.Description;
                taskFromDb.Status = taskObj.Status;
                taskFromDb.Priority = taskObj.Priority;
                taskFromDb.Severity = taskObj.Severity;
                taskFromDb.IsParent = taskObj.IsParent;
                taskFromDb.UserId = taskObj.UserId;
                taskFromDb.ClientId = taskObj.ClientId;
                taskFromDb.ParentTaskId = taskObj.ParentTaskId;
                taskFromDb.TaskId = taskObj.TaskId;
                taskFromDb.Duration = taskObj.Duration;

                _context.Tasks.Update(taskFromDb);
                _context.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong";
            }
        }
    }
}
