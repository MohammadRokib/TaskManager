using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.Services.Services.TaskService
{
    public partial class TaskService
    {
        public async Task<(PaginatedTaskDashboard?, string?)> GetDashBoardAsync(User request, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Tasks
                    .Where(t => t.UserId == request.Id)
                    .Include(t => t.Client)
                    .Include(t => t.User)
                    .OrderBy(t => t.TaskId);

                int totalRecords = await query.CountAsync();

                var pagedTasks = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
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
                

                var TasksList = new PaginatedTaskDashboard()
                {
                    Tasks = pagedTasks,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                };
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
