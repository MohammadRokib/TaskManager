using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels
{
    public class UpdateTaskViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double? Duration { get; set; }
        public Constants.TaskStatus Status { get; set; }
    }
}
