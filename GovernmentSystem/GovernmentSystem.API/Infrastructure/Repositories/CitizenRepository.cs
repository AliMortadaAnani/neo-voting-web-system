using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class CitizenRepository : ICitizenRepository
    {
        public void AddCitizen(Citizen citizen)
        {
            throw new NotImplementedException();
        }

        public void Delete(Citizen citizen)
        {
            throw new NotImplementedException();
        }

        public Task<Citizen?> GetCitizenByNationalIdAsync(int nationalId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Citizen>> GetCitizensPagedAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCitizensTotalCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
