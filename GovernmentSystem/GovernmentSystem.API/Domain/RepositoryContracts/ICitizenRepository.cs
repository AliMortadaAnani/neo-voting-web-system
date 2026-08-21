using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICitizenRepository
    {
        Task<List<Citizen>> GetCitizensPagedAsync(int pageNumber, int pageSize);

        Task<int> GetCitizensTotalCountAsync();//needed for pagination UX

        Task<Citizen?> GetCitizenByNationalIdAsync(int nationalId);

        void AddCitizen(Citizen citizen);

        void Delete(Citizen citizen);
    }
}
