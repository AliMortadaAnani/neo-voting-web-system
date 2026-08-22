using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        

        Task<List<Voter>> GetPagedAsync(int pageNumber, int pageSize);

        Task<int> CountAsync();//needed for pagination UX

        Task<Voter?> GetVoterByNationalIdAsync(string nationalId);

        Task<Voter?> GetVoterByHashedDataAsync(string hashedData);

        void Add(Voter voter);

        void Delete(Voter voter);

    }
}