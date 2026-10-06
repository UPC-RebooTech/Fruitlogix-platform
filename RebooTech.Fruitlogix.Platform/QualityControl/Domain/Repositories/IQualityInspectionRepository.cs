using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;

public interface IQualityInspectionRepository : IBaseRepository<QualityInspection>
{
    Task<QualityInspection?> FindByBatchIdAsync(int batchId);
}