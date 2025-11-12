using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            // Define the primary key
            builder.HasKey(e => e.Id);

            // Configure properties
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(250);

            // Configure relationships
            // An Election has one ElectionStatus.
            // An ElectionStatus can be associated with many Elections.
            builder.HasOne(e => e.ElectionStatus)
                .WithMany() // We don't need a collection of Elections on the ElectionStatus class
                .HasForeignKey(e => e.ElectionStatusId)
                .IsRequired();
        }
    }
}
