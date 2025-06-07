using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Constants;

namespace TaskManager.Models.Entities
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? Endtime { get; set; }
        public double? Duration { get; set; }
        public Constants.TaskStatus Status { get; set; }
        public Constants.Priority Priority { get; set; }
        public Constants.Severity Severity { get; set; }
        public bool IsParent { get; set; }
        public string UserId { get; set; }
        public int ClientId { get; set; }
        public int? ParentTaskId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Client Client { get; set; }
        public List<Models.Entities.Task>? SubTasks { get; set; }
    }
}
