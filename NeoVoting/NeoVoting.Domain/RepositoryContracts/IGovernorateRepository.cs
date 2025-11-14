using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IGovernorateRepository
    {
        Task<IReadOnlyList<Governorate>> GetAllGovernorates();
    }
}
