using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;

public interface ICommercialClientQueryService
{
    Task<CommercialClient?> Handle(GetCommercialClientByUserIdQuery query);
}
