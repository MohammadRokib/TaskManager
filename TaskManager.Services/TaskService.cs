using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Data;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;
using TaskManager.Services.IServices;

namespace TaskManager.Services
{
    public class TaskService(UserManager<User> userManager, TaskManagerDbContext context) : ITaskService
    {
        private readonly TaskManagerDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<(List<TaskDashboardViewModel>?,
                           List<ClientListViewModel>?,
                           List<ParentTaskListViewModel>?,
                           string?)> GetDashBoardAsync()
        {
            try
            {
                List<TaskDashboardViewModel> TasksList = await _context.Tasks
                    .Select(t => new TaskDashboardViewModel()
                    {
                        TaskTitle = t.Title,
                        ClientFullname = t.Client.ClientFullName,
                        IssueTime = t.StartTime,
                        AssignedTo = t.User.Name,
                        Status = t.Status
                    })
                    .ToListAsync();

                List<ClientListViewModel>? ClientsList = await _context.Clients
                    .Select(c => new ClientListViewModel()
                    {
                        Value = c.ClientId,
                        Text = c.ClientFullName
                    })
                    .ToListAsync();

                List<ParentTaskListViewModel>? ParentTasksList = await _context.Tasks
                    .Where(t => t.IsParent)
                    .Select(t => new ParentTaskListViewModel()
                    {
                        Value = t.TaskId,
                        Text = t.Title
                    })
                    .ToListAsync();

                return (TasksList, ClientsList, ParentTasksList, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (null, null, null, "Something went wrong");
            }
        }

        public async Task<string?> AddTaskAsync(AddTaskViewModel request)
        {
            Models.Entities.Task newTask = new Models.Entities.Task()
            {
                Title = request.Title,
                Description = request.Description,
                StartTime = DateTime.Now,
                Duration = 0.0,
                Status = Models.Constants.TaskStatus.New,
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
                _context.SaveChanges();
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
