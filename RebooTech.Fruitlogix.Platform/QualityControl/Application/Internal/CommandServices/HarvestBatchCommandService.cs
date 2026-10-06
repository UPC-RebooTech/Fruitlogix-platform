using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.CommandServices;

public class HarvestBatchCommandService(
    IHarvestBatchRepository harvestBatchRepository,
    IUnitOfWork unitOfWork
) : IHarvestBatchCommandService
{
    public async Task<HarvestBatch> Handle(CreateHarvestBatchCommand command)
    {
        var batch = new HarvestBatch(
            command.ProducerId,
            command.FruitType,
            command.QuantityKg,
            command.HarvestDate
        );

        await harvestBatchRepository.AddAsync(batch);
        await unitOfWork.CompleteAsync();

        return batch;
    }

    public async Task<HarvestBatch?> Handle(UpdateHarvestBatchStatusCommand command)
    {
        var batch = await harvestBatchRepository.FindByIdAsync(command.BatchId);
        if (batch is null) return null;

        batch.UpdateStatus(command.NewStatus);
        harvestBatchRepository.Update(batch);
        await unitOfWork.CompleteAsync();

        return batch;
    }
}