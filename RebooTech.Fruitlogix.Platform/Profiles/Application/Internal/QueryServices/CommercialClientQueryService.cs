using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Profiles.Application.Internal.QueryServices;

public class CommercialClientQueryService(ICommercialClientRepository commercialClientRepository)
    : ICommercialClientQueryService
{
    public Task<CommercialClient?> Handle(GetCommercialClientByUserIdQuery query) =>
        commercialClientRepository.FindByUserIdAsync(query.UserId);
}
