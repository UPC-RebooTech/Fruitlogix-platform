using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.CommandServices;

public class QualityInspectionCommandService(
    IQualityInspectionRepository qualityInspectionRepository,
    IUnitOfWork unitOfWork
) : IQualityInspectionCommandService
{
    public async Task<QualityInspection> Handle(CreateQualityInspectionCommand command)
    {
        var inspection = new QualityInspection(
            command.BatchId,
            command.Notes,
            new AppearanceRating(command.AppearanceRating),
            new VisualDefects(
                command.HasStains,
                command.HasBruises,
                command.HasDeformations,
                command.HasRot),
            new WastePercentage(command.WastePercentage),
            command.TemperatureCelsius,
            command.HumidityPercent,
            command.Ph,
            new BrixDegrees(command.BrixDegrees),
            command.DryCleaningDone,
            command.CaliberSortingConfirmed,
            command.PackagingMaterialInspected,
            command.FinalBoxSealing
        );

        await qualityInspectionRepository.AddAsync(inspection);
        await unitOfWork.CompleteAsync();   // 1er save: QualityInspection obtiene su Id

        inspection.BindSubEntities();       // fija FK correcto en sub-entities
        qualityInspectionRepository.Update(inspection);
        await unitOfWork.CompleteAsync();   // 2do save: persiste sub-entities con FK correcto

        return inspection;
    }
}