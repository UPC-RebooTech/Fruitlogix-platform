using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Infrastructure.Persistence.EFC.Repositories;

public class HarvestBatchRepository(AppDbContext context)
    : BaseRepository<HarvestBatch>(context), IHarvestBatchRepository
{
}