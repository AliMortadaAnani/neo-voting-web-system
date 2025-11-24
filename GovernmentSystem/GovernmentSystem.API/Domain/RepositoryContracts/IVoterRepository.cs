using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        Task<List<Voter>> GetAllVotersAsync();

        Task<Voter?> GetVoterByNationalIdAsync(Guid id);

        Task<Voter> AddVoterAsync(Voter voter);


        // Add these back. They are simple, synchronous persistence operations.
        void Update(Voter voter);
        void Delete(Voter voter);
    }
}
