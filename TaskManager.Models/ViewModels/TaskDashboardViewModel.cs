using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels
{
    public class TaskDashboardViewModel
    {
        public string TaskTitle { get; set; }
        public string ClientFullname { get; set; }
        public DateTime IssueTime { get; set; }
        public string AssignedTo { get; set; }
        public Models.Constants.TaskStatus Status { get; set; }
    }
}
