using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionRepository : IElectionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ElectionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken)
        {
            await _dbContext.Elections.AddAsync(election, cancellationToken);
            return election;
        }

        public async Task<List<Election>> GetAllElectionsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Elections.Include(e => e.ElectionStatus).ToListAsync(cancellationToken);
        }

        public async Task<Election?> GetElectionByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Elections.Include(e => e.ElectionStatus).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        // The simple, correct implementation for Update.
        public void Update(Election election)
        {
            // This single line handles the disconnected scenario perfectly.
            // It's a synchronous, in-memory operation.
            _dbContext.Elections.Update(election);// Marks the entity as Modified.
            // When SaveChangesAsync is called, EF Core will generate the appropriate SQL UPDATE statement. (needed for cases were the entity is disconnected/not fetched previously)
        }



        //Application logic moved to ElectionService
        /*public async Task<Election?> UpdateElectionDetailsAsync(Election election, CancellationToken cancellationToken)
        {
            Election? matchingElection = await GetElectionByIdAsync(election.Id, cancellationToken);
            if (matchingElection != null)
            {
                matchingElection.Update(
                    election.Name,
                    election.NominationStartDate,
                    election.NominationEndDate,
                    election.VotingStartDate,
                    election.VotingEndDate
                );
                return matchingElection;
            }
            else
            {
                return null;
            }
        }

        public async Task<Election?> UpdateElectionStatusAsync(Election election, ElectionStatusEnum updatedElectionStatus, CancellationToken cancellationToken)
        {
            Election? matchingElection = await GetElectionByIdAsync(election.Id, cancellationToken);
            if (matchingElection != null)
            {
                switch(updatedElectionStatus)
                {
                    case ElectionStatusEnum.Nomination:
                        matchingElection.StartNomination();
                        break;
                    case ElectionStatusEnum.Voting:
                        matchingElection.StartVoting();
                        break;
                    case ElectionStatusEnum.Completed:
                        matchingElection.CompleteElection();
                        break;
                }
                return matchingElection;
            }
            else
            {
                return null;
            }
        }*/

    }
}