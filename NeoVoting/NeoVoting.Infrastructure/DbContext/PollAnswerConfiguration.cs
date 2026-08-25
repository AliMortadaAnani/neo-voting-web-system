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
    public class PollAnswerConfiguration : IEntityTypeConfiguration<PollAnswer>
    {
        public void Configure(EntityTypeBuilder<PollAnswer> builder)
        {
            // Primary key
            builder.HasKey(v => v.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties
            builder.Property(v => v.Answer)
                .IsRequired()
                .HasMaxLength(4000)
                ;


          
            builder.HasOne(ew => ew.Poll)     
                .WithMany(cp => cp.PollAnswers)        
                .HasForeignKey(ew => ew.PollId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
