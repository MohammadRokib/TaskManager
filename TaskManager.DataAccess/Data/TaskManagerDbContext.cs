using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Data.Configuration;
using TaskManager.Models.Entities;

namespace TaskManager.DataAccess.Data
{
    public class TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<User> Uses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Models.Entities.Task> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
