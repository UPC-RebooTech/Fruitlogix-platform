using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Infrastructure.Persistence.EFC.Repositories;

public class AlertRepository : BaseRepository<Alert>, IAlertRepository
{
    public AlertRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Alert>> FindActiveAlertsAsync()
    {
        return await Context.Set<Alert>()
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}