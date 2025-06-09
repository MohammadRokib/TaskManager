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
    public class TaskService(UserManager<User> userManager, TaskManagerDbContext context, IClientService clientService) : ITaskService
    {
        private readonly TaskManagerDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IClientService _clientService = clientService;

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

        public async Task<(UpdateTaskViewModel?, string?)> GetTaskByIdAsync(int? taskId)
        {
            try
            {
                UpdateTaskViewModel? taskObj = await _context.Tasks
                .Where(t => t.TaskId == taskId)
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
                    UserId = t.UserId,
                    ParentTaskId = t.ParentTaskId
                })
                .FirstOrDefaultAsync();

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
