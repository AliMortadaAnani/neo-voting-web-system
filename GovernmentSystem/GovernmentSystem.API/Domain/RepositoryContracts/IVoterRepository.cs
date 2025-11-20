using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        Task<List<Voter>> GetAllVoters();

        Task<Voter?> GetVoterById(Guid id);

        Task<Voter> AddVoter(Voter voter);


        // Add these back. They are simple, synchronous persistence operations.
        void Update(Voter voter);
        void Delete(Voter voter);
    }
}
