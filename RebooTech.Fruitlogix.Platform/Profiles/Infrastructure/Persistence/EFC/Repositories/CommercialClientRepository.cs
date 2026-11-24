using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class CommercialClientRepository(AppDbContext context)
    : BaseRepository<CommercialClient>(context), ICommercialClientRepository
{
    public Task<CommercialClient?> FindByUserIdAsync(int userId) =>
        Context.Set<CommercialClient>().FirstOrDefaultAsync(c => c.UserId == userId);
}
