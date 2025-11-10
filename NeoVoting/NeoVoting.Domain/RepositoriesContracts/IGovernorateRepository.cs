using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoriesContracts
{
    public interface IGovernorateRepository
    {
        Task<IReadOnlyList<Governorate>> GetAllGovernorates();
    }
}
