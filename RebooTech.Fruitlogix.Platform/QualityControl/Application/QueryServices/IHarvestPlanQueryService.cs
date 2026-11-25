using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;

public interface IHarvestPlanQueryService
{
    Task<IEnumerable<HarvestPlan>> Handle(GetHarvestPlansQuery query);
    Task<HarvestPlan?> Handle(GetHarvestPlanByIdQuery query);
}
