namespace RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Resources;

/// <summary>Almacén del cliente: centro (lat/lng) y radio de la geocerca en km.</summary>
public record WarehouseLocationResource(double Latitude, double Longitude, double RadiusKm);

/// <summary>Perfil del cliente comercial. warehouse es null hasta que se configure.</summary>
public record CommercialClientProfileResource(
    int Id,
    int UserId,
    string? BusinessName,
    string? Ruc,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address,
    WarehouseLocationResource? Warehouse,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

/// <summary>
///     PUT /profiles/me: reemplaza los datos de contacto (un campo que no se envía queda vacío).
///     warehouse es opcional: si no se envía, se conserva el almacén actual.
/// </summary>
/// <param name="Ruc">11 dígitos.</param>
public record UpdateCommercialClientProfileResource(
    string? BusinessName = null,
    string? Ruc = null,
    string? ContactName = null,
    string? Email = null,
    string? Phone = null,
    string? Address = null,
    WarehouseLocationResource? Warehouse = null
);
