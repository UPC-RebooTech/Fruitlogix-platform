using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;

public interface IProducerQueryService
{
    Task<IEnumerable<Producer>> Handle(GetAllProducersQuery query);
    
    Task<Producer?> Handle(GetProducerByIdQuery query);

}