using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Geofencing;

/// <summary>
///     Geocerca del almacén del cliente (US65): si una lectura GPS de una entrega en tránsito
///     cae dentro del radio del almacén del cliente del pedido, la entrega pasa a Arrived.
/// </summary>
public interface IWarehouseGeofenceService
{
    /// <summary>true si la lectura marcó la llegada. Nunca lanza: un fallo no debe romper el tracking.</summary>
    Task<bool> CheckArrivalAsync(int deliveryId, double latitude, double longitude);
}

public class WarehouseGeofenceService(
    AppDbContext context,
    IDeliveryCommandService deliveryCommandService,
    ILogger<WarehouseGeofenceService> logger) : IWarehouseGeofenceService
{
    public async Task<bool> CheckArrivalAsync(int deliveryId, double latitude, double longitude)
    {
        // (0,0) = sin señal GPS
        if (latitude == 0 && longitude == 0) return false;

        try
        {
            var delivery = await context.Set<Delivery>().AsNoTracking()
                .Where(d => d.Id == deliveryId)
                .Select(d => new { d.OrderId, d.CurrentStatus })
                .FirstOrDefaultAsync();
            if (delivery is null
                || delivery.CurrentStatus is not (DeliveryStatus.InTransit or DeliveryStatus.Delayed))
                return false;

            var clientId = await context.Set<Order>().AsNoTracking()
                .Where(o => o.Id == delivery.OrderId)
                .Select(o => (int?)o.CommercialClientId)
                .FirstOrDefaultAsync();
            if (clientId is null) return false;

            var warehouse = await context.Set<CommercialClient>().AsNoTracking()
                .Where(c => c.UserId == clientId)
                .Select(c => new { c.WarehouseLatitude, c.WarehouseLongitude, c.WarehouseRadiusKm })
                .FirstOrDefaultAsync();
            if (warehouse?.WarehouseLatitude is not { } warehouseLat
                || warehouse.WarehouseLongitude is not { } warehouseLng
                || warehouse.WarehouseRadiusKm is not { } radiusKm)
                return false;

            var distanceKm = GeoDistance.HaversineKm(latitude, longitude, warehouseLat, warehouseLng);
            if (distanceKm > radiusKm) return false;

            await deliveryCommandService.Handle(new ArriveDeliveryCommand(deliveryId, ActorUserId: null,
                latitude, longitude, $"Detected by warehouse geofence ({distanceKm:0.00} km from the warehouse)."));
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Geofence check failed for delivery {DeliveryId}", deliveryId);
            return false;
        }
    }
}

public static class GeoDistance
{
    private const double EarthRadiusKm = 6371.0088;

    /// <summary>Distancia en km entre dos puntos (fórmula de Haversine).</summary>
    public static double HaversineKm(double lat1, double lng1, double lat2, double lng2)
    {
        static double ToRadians(double degrees) => degrees * Math.PI / 180;
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return 2 * EarthRadiusKm * Math.Asin(Math.Min(1, Math.Sqrt(a)));
    }
}
