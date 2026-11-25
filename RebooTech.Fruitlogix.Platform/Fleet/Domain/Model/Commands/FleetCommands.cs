namespace RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;

/// <param name="LicenseClass">A-IIb, A-IIIa, A-IIIb o A-IIIc.</param>
/// <param name="Status">Available (por defecto), OnRoute u OffDuty.</param>
public record CreateDriverCommand(
    string Name,
    string? Phone,
    string LicenseClass,
    DateOnly LicenseExpiry,
    bool ColdChainCertified,
    string? Status,
    DateTimeOffset? RestUntil,
    int? CreatedByUserId
);

/// <summary>Reemplaza los datos del chofer. Status null conserva el actual; RestUntil null lo borra.</summary>
public record UpdateDriverCommand(
    int DriverId,
    string Name,
    string? Phone,
    string LicenseClass,
    DateOnly LicenseExpiry,
    bool ColdChainCertified,
    string? Status,
    DateTimeOffset? RestUntil
);

public record DeleteDriverCommand(int DriverId);

/// <param name="Plate">Formato ABC-123 (único).</param>
/// <param name="Type">HeavyReefer, ReeferTruck, MediumReeferVan o LightReefer.</param>
/// <param name="Status">Available (por defecto), OnRoute o Maintenance.</param>
public record CreateVehicleCommand(
    string UnitCode,
    string Plate,
    string Type,
    double CapacityKg,
    double MinCelsius,
    double MaxCelsius,
    string? Status,
    int? CreatedByUserId
);

/// <summary>Reemplaza los datos del vehículo. Status null conserva el actual.</summary>
public record UpdateVehicleCommand(
    int VehicleId,
    string UnitCode,
    string Plate,
    string Type,
    double CapacityKg,
    double MinCelsius,
    double MaxCelsius,
    string? Status
);

public record DeleteVehicleCommand(int VehicleId);
