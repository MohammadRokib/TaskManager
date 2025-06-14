﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Entities
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public List<Task>? AssignedTasks { get; set; }
    }
}
