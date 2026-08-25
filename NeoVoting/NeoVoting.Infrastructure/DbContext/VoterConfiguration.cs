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
    public class VoterConfiguration : IEntityTypeConfiguration<Voter>
    {
        public void Configure(EntityTypeBuilder<Voter> builder)
        {
            // Primary key
            builder.HasKey(v => v.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties
            builder.Property(v => v.FirstName)
                .IsRequired()
                .HasMaxLength(100); // adjust as needed

            builder.Property(v => v.LastName)
                .IsRequired()
                .HasMaxLength(100); // adjust as needed

            builder.Property(v => v.DateOfBirth)
                .IsRequired();


            builder.Property(v => v.Governorate)
                .IsRequired()
                .HasConversion<int>(); // Store enum as int in the database

            // 1. Get all integer values from the Enum
            var enumValues = Enum.GetValues(typeof(GovernorateIdEnum))
                                 .Cast<int>();

            // 2. Create the SQL string: "1, 2, 3"
            var sqlValues = string.Join(", ", enumValues);

            // 3. Add the Check Constraint
            // SQL: CHECK ([GovernorateId] IN (1, 2, 3) )
            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Voter_Governorate", $"([Governorate] IN ({sqlValues}) )")
            );


            builder.Property(v => v.Gender)
              .IsRequired()
              .HasMaxLength(1);

            builder.ToTable(tb =>
               tb.HasCheckConstraint("CK_Voter_Gender", "[Gender] IN ('M', 'F')"));


            builder.Property(v => v.VerificationHash)
                .IsRequired()
                .HasMaxLength(200); // adjust as needed



            builder.HasOne(ew => ew.User)
                .WithOne(cp => cp.Voter)
                .HasForeignKey<Voter>(ew => ew.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ew => new { ew.UserId }).IsUnique();


        }
    }
}
