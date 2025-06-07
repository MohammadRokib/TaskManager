using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Entities;

namespace TaskManager.DataAccess.Data.Configuration
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");
            builder.HasKey(c => c.ClientId);
            builder.Property(c => c.ClientFullName)
                .HasMaxLength(150)
                .IsRequired();
            builder.HasIndex(c => c.ClientFullName)
                .IsUnique();
            builder.Property(c => c.ClientShortName)
                .HasMaxLength(15)
                .IsRequired();
            builder.HasIndex(c => c.ClientShortName);
            builder.Property(c => c.ClientAddress)
                .HasMaxLength(100);
            builder.HasMany(c => c.OrderedTask)
                .WithOne(t => t.Client)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
