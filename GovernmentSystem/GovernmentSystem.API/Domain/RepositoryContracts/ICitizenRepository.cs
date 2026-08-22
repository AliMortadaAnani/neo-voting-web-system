using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICitizenRepository
    {
        Task<List<Citizen>> GetPagedAsync(int pageNumber, int pageSize);

        Task<int> CountAsync();//needed for pagination UX

        Task<Citizen?> GetCitizenByNationalIdAsync(string nationalId);

        void Add(Citizen citizen);

        void Delete(Citizen citizen);
    }
}
