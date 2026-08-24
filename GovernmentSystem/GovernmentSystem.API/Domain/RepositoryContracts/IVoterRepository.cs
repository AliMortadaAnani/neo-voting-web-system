using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        Task<Voter?> GetVoterByNationalIdAsync(string nationalId);

        Task<List<Voter>> GetPagedAsync(int pageNumber, int pageSize);

        Task<Voter?> GetVoterByHashedDataAsync(string hashedData);

        Task<bool> IsVoterExistByNationalIdAsync(string nationalId);

        Task<int> CountAsync();//needed for pagination UX

        void Add(Voter voter);

        void Delete(Voter voter);
    }
}