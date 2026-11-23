using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Infrastructure.Persistence.EFC.Repositories;

public class MembershipRepository(AppDbContext context)
    : BaseRepository<Membership>(context), IMembershipRepository
{
    public async Task<Membership?> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        await Context.Set<Membership>().FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
}
