using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.CommandServices;

public class IncidentCommandService(
    IIncidentRepository incidentRepository,
    IUnitOfWork unitOfWork
) : IIncidentCommandService
{
    public async Task<Incident> Handle(CreateIncidentCommand command)
    {
        var incident = new Incident(
            command.BatchId,
            command.Description,
            command.EvidenceUrls
        );

        await incidentRepository.AddAsync(incident);
        await unitOfWork.CompleteAsync();

        return incident;
    }
    
    public async Task<Incident?> Handle(UpdateIncidentStatusCommand command)
    {
        var incident = await incidentRepository.FindByIdAsync(command.IncidentId);
        if (incident is null) return null;

        incident.UpdateStatus(command.NewStatus);
        incidentRepository.Update(incident);
        await unitOfWork.CompleteAsync();

        return incident;
    }
    
    
}