using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        

        Task<List<Voter>> GetVotersPagedAsync(int pageNumber, int pageSize);

        Task<int> GetVotersTotalCountAsync();//needed for pagination UX

        Task<Voter?> GetVoterByNationalIdAsync(string nationalId);

        Task<Voter?> GetVoterByHashedDataAsync(string hashedData);

        void AddVoter(Voter voter);

        void Delete(Voter voter);

    }
}