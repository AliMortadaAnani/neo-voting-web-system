using NeoVoting.Domain.Entities;
using System.Diagnostics.Metrics;

namespace NeoVoting.Domain.RepositoriesContracts
{
    public interface IGovernorateRepository
    {
        Task<List<Governorate>> GetAllGovernorates();
    }
}
