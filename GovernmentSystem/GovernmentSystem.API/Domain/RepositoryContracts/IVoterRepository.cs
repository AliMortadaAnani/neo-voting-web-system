using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        Task<List<Voter>> GetAllVotersAsync();//Not recommended because of huge number of records retrieved

        Task<List<Voter>> GetPagedVotersStoredProcAsync(int skip, int take);//we implemented this method using a stored procedure saved in migrations and called by Ef core

        Task<int> GetTotalVotersCountAsync();//needed for pagination

        Task<Voter?> GetVoterByNationalIdAsync(Guid nationalId);

        Task<Voter> AddVoterAsync(Voter voter);

        Task ResetAllVotedFieldToFalse();

        //Update and Delete operations in Ef core are simple, synchronous persistence operations.
        //So, we don't need to make them async methods here. -> no Task<>
        void Update(Voter voter);

        void Delete(Voter voter);
    }
}