using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Transform;

public static class CommercialClientProfileResourceAssembler
{
    public static CommercialClientProfileResource ToResourceFromEntity(CommercialClient client) =>
        new(
            client.Id,
            client.UserId,
            client.BusinessName,
            client.Ruc,
            client.ContactName,
            client.Email,
            client.Phone,
            client.Address,
            client.HasWarehouse
                ? new WarehouseLocationResource(client.WarehouseLatitude!.Value, client.WarehouseLongitude!.Value,
                    client.WarehouseRadiusKm!.Value)
                : null,
            client.CreatedAt,
            client.UpdatedAt
        );

    public static UpdateCommercialClientProfileCommand ToCommandFromResource(int userId,
        UpdateCommercialClientProfileResource resource) =>
        new(userId, resource.BusinessName, resource.Ruc, resource.ContactName, resource.Email, resource.Phone,
            resource.Address);

    public static SetWarehouseLocationCommand ToCommandFromResource(int userId, WarehouseLocationResource resource) =>
        new(userId, resource.Latitude, resource.Longitude, resource.RadiusKm);
}
