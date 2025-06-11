using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Entities;

namespace TaskManager.Models.ViewModels
{
    public class UpdateTaskViewModel : AddTaskViewModel
    {
        public int TaskId { get; set; }
        public double? Duration { get; set; }
        public Client? Client { get; set; }
        public ParentTaskListViewModel? ParentTask { get; set; }
    }
}
