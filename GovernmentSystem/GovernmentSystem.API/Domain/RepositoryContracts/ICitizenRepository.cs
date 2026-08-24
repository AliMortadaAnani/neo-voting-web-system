using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICitizenRepository
    {
        Task<Citizen?> GetCitizenByNationalIdAsync(string nationalId);

        Task<List<Citizen>> GetPagedAsync(int pageNumber, int pageSize);

        Task<int> CountAsync();//needed for pagination UX

        void Add(Citizen citizen);

        void Delete(Citizen citizen);
    }
}