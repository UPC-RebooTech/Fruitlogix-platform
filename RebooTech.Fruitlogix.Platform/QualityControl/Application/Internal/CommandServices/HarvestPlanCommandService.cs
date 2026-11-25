using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.CommandServices;

public class HarvestPlanCommandService(IHarvestPlanRepository harvestPlanRepository, IUnitOfWork unitOfWork)
    : IHarvestPlanCommandService
{
    public async Task<HarvestPlan> Handle(SaveHarvestPlanCommand command)
    {
        var plan = new HarvestPlan(command);
        await harvestPlanRepository.AddAsync(plan);
        await unitOfWork.CompleteAsync();
        return plan;
    }

    public async Task<HarvestPlan?> Handle(UpdateHarvestPlanCommand command)
    {
        var plan = await harvestPlanRepository.FindByIdAsync(command.HarvestPlanId);
        if (plan is null) return null;
        plan.Update(command.Data);
        harvestPlanRepository.Update(plan);
        await unitOfWork.CompleteAsync();
        return plan;
    }

    public async Task<bool> Handle(DeleteHarvestPlanCommand command)
    {
        var plan = await harvestPlanRepository.FindByIdAsync(command.HarvestPlanId);
        if (plan is null) return false;
        harvestPlanRepository.Remove(plan);
        await unitOfWork.CompleteAsync();
        return true;
    }
}
