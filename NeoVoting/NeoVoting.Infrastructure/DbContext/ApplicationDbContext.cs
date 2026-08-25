using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.IdentityEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Domain Tables
        public DbSet<Election> Elections { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollAnswer> PollAnswers { get; set; }

        public DbSet<PollVote> PollVotes { get; set; }

        public DbSet<EventParticipation> EventParticipations { get; set; }

        public DbSet <Voter> Voters { get; set; }

        public DbSet <Candidate> Candidates { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteChoice> VoteChoices { get; set; }
        public DbSet<ElectionWinner> ElectionWinners { get; set; }
   
        public DbSet<SystemAuditLog> SystemAuditLogs { get; set; }

        public DbSet<ElectionAndPollStatistics> ElectionRegisteredVotersPerGovernorates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Essential for Identity framework keys and tables
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

     
        

    }
}