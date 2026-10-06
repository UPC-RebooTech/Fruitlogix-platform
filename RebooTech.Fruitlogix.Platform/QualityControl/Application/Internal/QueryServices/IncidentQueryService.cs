using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.QueryServices;

public class IncidentQueryService(
    IIncidentRepository incidentRepository
) : IIncidentQueryService
{
    public async Task<IEnumerable<Incident>> Handle(GetAllIncidentsQuery query) =>
        await incidentRepository.ListAsync();
}