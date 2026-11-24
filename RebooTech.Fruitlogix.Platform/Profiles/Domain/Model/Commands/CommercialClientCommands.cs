namespace RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;

/// <summary>Crea el perfil si no existe y reemplaza sus datos de contacto.</summary>
public record UpdateCommercialClientProfileCommand(
    int UserId,
    string? BusinessName,
    string? Ruc,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address
);

/// <summary>Crea el perfil si no existe y fija la geocerca del almacén.</summary>
public record SetWarehouseLocationCommand(int UserId, double Latitude, double Longitude, double RadiusKm);
