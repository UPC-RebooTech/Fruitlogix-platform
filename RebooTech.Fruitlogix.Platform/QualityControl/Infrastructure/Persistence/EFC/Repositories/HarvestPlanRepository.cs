using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Infrastructure.Persistence.EFC.Repositories;

public class HarvestPlanRepository(AppDbContext context)
    : BaseRepository<HarvestPlan>(context), IHarvestPlanRepository
{
    public async Task<IEnumerable<HarvestPlan>> FindAsync(int? producerId)
    {
        var plans = Context.Set<HarvestPlan>().AsQueryable();
        if (producerId is { } id) plans = plans.Where(p => p.ProducerId == id);
        return await plans.OrderBy(p => p.ScheduledFor).ThenBy(p => p.Id).ToListAsync();
    }
}
