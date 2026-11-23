using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;

public interface IMembershipRepository : IBaseRepository<Membership>
{
    Task<Membership?> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
