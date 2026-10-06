using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;

public interface IQualityInspectionCommandService
{
    Task<QualityInspection> Handle(CreateQualityInspectionCommand command);
}