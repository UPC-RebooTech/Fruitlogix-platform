using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;

public interface ICommercialClientRepository : IBaseRepository<CommercialClient>
{
    Task<CommercialClient?> FindByUserIdAsync(int userId);
}
