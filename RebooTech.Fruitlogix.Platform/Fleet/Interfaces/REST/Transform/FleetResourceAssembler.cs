using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Fleet.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.Fleet.Interfaces.REST.Transform;

public static class FleetResourceAssembler
{
    public static DriverResource ToResourceFromEntity(Driver d) =>
        new(d.Id, d.Name, d.Phone, d.LicenseClass, d.LicenseExpiry, d.ColdChainCertified, d.Status.ToString(),
            d.RestUntil, d.CreatedByUserId, d.CreatedAt, d.UpdatedAt);

    public static VehicleResource ToResourceFromEntity(Vehicle v) =>
        new(v.Id, v.UnitCode, v.Plate, v.Type.ToString(), v.CapacityKg, v.MinCelsius, v.MaxCelsius, v.Status.ToString(),
            v.CreatedByUserId, v.CreatedAt, v.UpdatedAt);

    public static CreateDriverCommand ToCreateCommand(SaveDriverResource r, int userId) =>
        new(r.Name, r.Phone, r.LicenseClass, r.LicenseExpiry, r.ColdChainCertified, r.Status, r.RestUntil, userId);

    public static UpdateDriverCommand ToUpdateCommand(int driverId, SaveDriverResource r) =>
        new(driverId, r.Name, r.Phone, r.LicenseClass, r.LicenseExpiry, r.ColdChainCertified, r.Status, r.RestUntil);

    public static CreateVehicleCommand ToCreateCommand(SaveVehicleResource r, int userId) =>
        new(r.UnitCode, r.Plate, r.Type, r.CapacityKg, r.MinCelsius, r.MaxCelsius, r.Status, userId);

    public static UpdateVehicleCommand ToUpdateCommand(int vehicleId, SaveVehicleResource r) =>
        new(vehicleId, r.UnitCode, r.Plate, r.Type, r.CapacityKg, r.MinCelsius, r.MaxCelsius, r.Status);
}
