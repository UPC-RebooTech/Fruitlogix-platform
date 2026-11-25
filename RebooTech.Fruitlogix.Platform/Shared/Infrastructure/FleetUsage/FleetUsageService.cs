using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.FleetUsage;

/// <summary>
///     Consulta en Logistics si un chofer o vehículo de la flota está asignado a una entrega activa
///     (PendingDispatch, InTransit, Delayed o Arrived). Se usa para bloquear su borrado (409).
/// </summary>
public interface IFleetUsageService
{
    Task<int?> FindActiveDeliveryForDriverAsync(int driverId);
    Task<int?> FindActiveDeliveryForVehicleAsync(int vehicleId);
}

public class FleetUsageService(AppDbContext context) : IFleetUsageService
{
    private static readonly DeliveryStatus[] ActiveStatuses =
        [DeliveryStatus.PendingDispatch, DeliveryStatus.InTransit, DeliveryStatus.Delayed, DeliveryStatus.Arrived];

    public Task<int?> FindActiveDeliveryForDriverAsync(int driverId) =>
        context.Set<Delivery>().AsNoTracking()
            .Where(d => d.DriverId == driverId && ActiveStatuses.Contains(d.CurrentStatus))
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync();

    public Task<int?> FindActiveDeliveryForVehicleAsync(int vehicleId) =>
        context.Set<Delivery>().AsNoTracking()
            .Where(d => d.VehicleId == vehicleId && ActiveStatuses.Contains(d.CurrentStatus))
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync();
}
