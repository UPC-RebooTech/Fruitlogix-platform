using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;

public interface IIncidentCommandService
{
    Task<Incident> Handle(CreateIncidentCommand command);
    Task<Incident?> Handle(UpdateIncidentStatusCommand command);  // ← nuevo
}