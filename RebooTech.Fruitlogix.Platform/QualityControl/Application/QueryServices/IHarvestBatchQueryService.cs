using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;

public interface IHarvestBatchQueryService
{
    Task<IEnumerable<HarvestBatch>> Handle(GetAllHarvestBatchesQuery query);
}