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
    public class PollStatisticsConfiguration : IEntityTypeConfiguration<PollStatistics>
    {
        public void Configure(EntityTypeBuilder<PollStatistics> builder)
        {


            // Primary key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();




           


            builder.HasOne(e => e.Poll)
                .WithOne(e => e.PollStatistics)
                .HasForeignKey<PollStatistics>(e => e.PollId)
                .IsRequired() 
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
