using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Infrastructure.DbContext
{
    public class PollVoteConfiguration : IEntityTypeConfiguration<PollVote>
    {
        public void Configure(EntityTypeBuilder<PollVote> builder)
        {
            // Primary key
            builder.HasKey(v => v.Id);
            builder.Property(entity => entity.Id).ValueGeneratedNever();
          

            builder.Property(v => v.TimestampUTC)
                .IsRequired();

            // Relationships
            builder.HasOne(v => v.Poll)
                .WithMany(e => e.PollVotes)
                .HasForeignKey(v => v.PollId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vc => vc.PollAnswer)
               .WithMany(cp => cp.PollVotes)
               .HasForeignKey(vc => vc.PollAnswerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
           

            builder.HasIndex(vc => new { vc.Id, vc.PollAnswerId })
              .IsUnique();


        }
    }
}
