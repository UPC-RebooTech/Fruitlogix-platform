using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;

public interface IIncidentQueryService
{
    Task<IEnumerable<Incident>> Handle(GetAllIncidentsQuery query);
}