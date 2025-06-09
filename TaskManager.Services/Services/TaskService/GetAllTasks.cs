using Microsoft.EntityFrameworkCore;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.Services.TaskService
{
    public partial class TaskService
    {
        public async Task<(List<TaskDashboardViewModel>?, string?)> GetDashBoardAsync()
        {
            try
            {
                List<TaskDashboardViewModel> TasksList = await _context.Tasks
                    .Select(t => new TaskDashboardViewModel()
                    {
                        TaskId = t.TaskId,
                        TaskTitle = t.Title,
                        ClientFullname = t.Client.ClientFullName,
                        IssueTime = t.StartTime,
                        AssignedTo = t.User.Name,
                        Status = t.Status
                    })
                    .ToListAsync();

                return (TasksList, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (null, "Something went wrong");
            }
        }
    }
}
