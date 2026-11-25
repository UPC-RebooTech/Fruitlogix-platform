using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;

namespace RebooTech.Fruitlogix.Platform.Fleet.Interfaces.REST.Resources;

/// <summary>Chofer. licenseClass: A-IIb, A-IIIa, A-IIIb o A-IIIc.</summary>
public record DriverResource(
    int Id,
    string Name,
    string? Phone,
    string LicenseClass,
    DateOnly LicenseExpiry,
    bool ColdChainCertified,
    [property: SwaggerEnumValues(typeof(DriverStatus))] string Status,
    DateTimeOffset? RestUntil,
    int? CreatedByUserId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

/// <summary>POST /drivers y PUT /drivers/{id}. En PUT, status null conserva el actual y restUntil null lo borra.</summary>
/// <param name="LicenseClass">A-IIb, A-IIIa, A-IIIb o A-IIIc.</param>
/// <param name="LicenseExpiry">yyyy-MM-dd.</param>
public record SaveDriverResource(
    string Name,
    string LicenseClass,
    DateOnly LicenseExpiry,
    bool ColdChainCertified = false,
    string? Phone = null,
    [property: SwaggerEnumValues(typeof(DriverStatus))] string? Status = null,
    DateTimeOffset? RestUntil = null
);

/// <summary>Vehículo refrigerado. plate: ABC-123.</summary>
public record VehicleResource(
    int Id,
    string UnitCode,
    string Plate,
    [property: SwaggerEnumValues(typeof(VehicleType))] string Type,
    double CapacityKg,
    double MinCelsius,
    double MaxCelsius,
    [property: SwaggerEnumValues(typeof(VehicleStatus))] string Status,
    int? CreatedByUserId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

/// <summary>POST /vehicles y PUT /vehicles/{id}. En PUT, status null conserva el actual.</summary>
/// <param name="Plate">Formato ABC-123, único (409 si se repite).</param>
/// <param name="CapacityKg">500 a 40000.</param>
/// <param name="MinCelsius">-30 a 30, menor o igual que maxCelsius.</param>
/// <param name="MaxCelsius">-30 a 30.</param>
public record SaveVehicleResource(
    string UnitCode,
    string Plate,
    [property: SwaggerEnumValues(typeof(VehicleType))] string Type,
    double CapacityKg,
    double MinCelsius,
    double MaxCelsius,
    [property: SwaggerEnumValues(typeof(VehicleStatus))] string? Status = null
);
