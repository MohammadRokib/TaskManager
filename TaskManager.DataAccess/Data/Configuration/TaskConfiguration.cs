using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Data.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<Models.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Models.Entities.Task> builder)
        {
            builder.ToTable("Tasks");
            builder.HasKey(t => t.TaskId);
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(1000);
            builder.Property(t => t.Status)
                .HasDefaultValue(Models.Constants.TaskStatus.New)
                .IsRequired();
            builder.Property(t => t.IsParent)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.ParentTaskId);
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.ClientId);

            builder.HasMany(t => t.SubTasks)
                .WithOne()
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.User)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(t => t.Client)
                .WithMany(c => c.OrderedTask)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
