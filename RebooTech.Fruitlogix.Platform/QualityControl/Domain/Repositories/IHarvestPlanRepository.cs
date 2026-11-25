using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;

public interface IHarvestPlanRepository : IBaseRepository<HarvestPlan>
{
    Task<IEnumerable<HarvestPlan>> FindAsync(int? producerId);
}
