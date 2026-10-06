using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.QueryServices;

public class QualityInspectionQueryService(
    IQualityInspectionRepository qualityInspectionRepository
) : IQualityInspectionQueryService
{
    public async Task<QualityInspection?> Handle(GetQualityInspectionByBatchIdQuery query) =>
        await qualityInspectionRepository.FindByBatchIdAsync(query.BatchId);
}