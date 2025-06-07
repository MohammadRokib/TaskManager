using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels
{
    public class ClientDashboardViewModel
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public int OrderedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}
