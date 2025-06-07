using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Entities
{
    public class Client
    {
        public int ClientId { get; set; }
        public string ClientFullName { get; set; }
        public string ClientShortName { get; set; }
        public string ClientAddress { get; set; }
        public List<Task>? OrderedTask { get; set; }
    }
}
