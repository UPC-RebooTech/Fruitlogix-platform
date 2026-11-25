using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;

public interface IHarvestPlanCommandService
{
    Task<HarvestPlan> Handle(SaveHarvestPlanCommand command);
    Task<HarvestPlan?> Handle(UpdateHarvestPlanCommand command);
    /// <summary>false si no existe.</summary>
    Task<bool> Handle(DeleteHarvestPlanCommand command);
}
