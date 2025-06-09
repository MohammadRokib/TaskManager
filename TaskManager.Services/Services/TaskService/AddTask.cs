using TaskManager.Models.ViewModels;

namespace TaskManager.Services.Services.TaskService
{
    public partial class TaskService
    {
        public async Task<string?> AddTaskAsync(AddTaskViewModel request)
        {
            Models.Entities.Task newTask = new Models.Entities.Task()
            {
                Title = request.Title,
                Description = request.Description,
                StartTime = DateTime.Now,
                Duration = 0.0,
                Status = Models.Constants.TaskStatus.New,
                Priority = request.Priority,
                Severity = request.Severity,
                IsParent = request.IsParent,
                UserId = request.UserId,
                ClientId = request.ClientId
            };

            if (!request.IsParent)
            {
                newTask.ParentTaskId = request.ParentTaskId;
            }

            try
            {
                await _context.Tasks.AddAsync(newTask);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "Something went wrong please try again";
            }

            return null;
        }
    }
}
