using RebooTech.Fruitlogix.Platform.Profiles.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Profiles.Application.Internal.CommandServices;

public class CommercialClientCommandService(
    ICommercialClientRepository commercialClientRepository,
    IUnitOfWork unitOfWork) : ICommercialClientCommandService
{
    public async Task<CommercialClient> Handle(UpdateCommercialClientProfileCommand command)
    {
        var (client, isNew) = await FindOrCreateAsync(command.UserId);
        client.UpdateDetails(command);
        await SaveAsync(client, isNew);
        return client;
    }

    public async Task<CommercialClient> Handle(SetWarehouseLocationCommand command)
    {
        var (client, isNew) = await FindOrCreateAsync(command.UserId);
        client.SetWarehouse(command.Latitude, command.Longitude, command.RadiusKm);
        await SaveAsync(client, isNew);
        return client;
    }

    private async Task<(CommercialClient Client, bool IsNew)> FindOrCreateAsync(int userId)
    {
        var existing = await commercialClientRepository.FindByUserIdAsync(userId);
        return existing is null ? (new CommercialClient(userId), true) : (existing, false);
    }

    private async Task SaveAsync(CommercialClient client, bool isNew)
    {
        if (isNew) await commercialClientRepository.AddAsync(client);
        else commercialClientRepository.Update(client);
        await unitOfWork.CompleteAsync();
    }
}
