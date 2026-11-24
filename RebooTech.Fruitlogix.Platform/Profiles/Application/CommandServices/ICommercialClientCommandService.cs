using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.Profiles.Application.CommandServices;

public interface ICommercialClientCommandService
{
    Task<CommercialClient> Handle(UpdateCommercialClientProfileCommand command);
    Task<CommercialClient> Handle(SetWarehouseLocationCommand command);
}
