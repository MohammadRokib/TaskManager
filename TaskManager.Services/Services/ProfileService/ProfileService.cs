using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Data;
using TaskManager.Models.Entities;
using TaskManager.Services.IServices;

namespace TaskManager.Services.Services.ProfileService
{
    public class ProfileService(TaskManagerDbContext context, UserManager<User> userManager) : IProfileService
    {
        private readonly TaskManagerDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        public async void GetCurrentUser()
        {
        }
    }
}
