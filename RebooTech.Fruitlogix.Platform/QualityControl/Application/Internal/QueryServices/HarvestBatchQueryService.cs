using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.QueryServices;

public class HarvestBatchQueryService(
    IHarvestBatchRepository harvestBatchRepository
) : IHarvestBatchQueryService
{
    public async Task<IEnumerable<HarvestBatch>> Handle(GetAllHarvestBatchesQuery query) =>
        await harvestBatchRepository.ListAsync();
}