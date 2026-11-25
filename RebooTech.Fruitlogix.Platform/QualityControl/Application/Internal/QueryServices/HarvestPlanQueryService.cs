using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.QueryServices;

public class HarvestPlanQueryService(IHarvestPlanRepository harvestPlanRepository) : IHarvestPlanQueryService
{
    public Task<IEnumerable<HarvestPlan>> Handle(GetHarvestPlansQuery query) =>
        harvestPlanRepository.FindAsync(query.ProducerId);

    public Task<HarvestPlan?> Handle(GetHarvestPlanByIdQuery query) =>
        harvestPlanRepository.FindByIdAsync(query.HarvestPlanId);
}
