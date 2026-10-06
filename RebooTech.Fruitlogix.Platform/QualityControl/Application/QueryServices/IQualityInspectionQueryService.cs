using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;

public interface IQualityInspectionQueryService
{
    Task<QualityInspection?> Handle(GetQualityInspectionByBatchIdQuery query);
}