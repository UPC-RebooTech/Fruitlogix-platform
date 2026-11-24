using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;

/// <summary>
///     Perfil del cliente comercial (uno por usuario IAM con rol CommercialClient).
///     Incluye la ubicación del almacén que se usa como geocerca de llegada.
/// </summary>
public class CommercialClient : IAuditableEntity
{
    public const int MaxBusinessNameLength = 150;
    public const int MaxRucLength = 11;
    public const int MaxContactNameLength = 100;
    public const int MaxEmailLength = 100;
    public const int MaxPhoneLength = 30;
    public const int MaxAddressLength = 255;
    public const double MaxWarehouseRadiusKm = 50;

    public int Id { get; private set; }
    /// <summary>Id del usuario IAM (= clientId de los pedidos).</summary>
    public int UserId { get; private set; }
    /// <summary>Razón social.</summary>
    public string? BusinessName { get; private set; }
    /// <summary>RUC de 11 dígitos.</summary>
    public string? Ruc { get; private set; }
    public string? ContactName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public double? WarehouseLatitude { get; private set; }
    public double? WarehouseLongitude { get; private set; }
    public double? WarehouseRadiusKm { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>true si el almacén tiene coordenadas y radio.</summary>
    public bool HasWarehouse => WarehouseLatitude is not null && WarehouseLongitude is not null && WarehouseRadiusKm is not null;

    protected CommercialClient() { }

    public CommercialClient(int userId)
    {
        if (userId <= 0) throw new ArgumentException("UserId must be a positive integer.");
        UserId = userId;
    }

    public void UpdateDetails(UpdateCommercialClientProfileCommand command)
    {
        BusinessName = Optional(command.BusinessName, "businessName", MaxBusinessNameLength);
        Ruc = Optional(command.Ruc, "ruc", MaxRucLength);
        if (Ruc is not null && (Ruc.Length != 11 || !Ruc.All(char.IsAsciiDigit)))
            throw new ArgumentException("RUC must have exactly 11 digits.", "ruc");
        ContactName = Optional(command.ContactName, "contactName", MaxContactNameLength);
        Email = Optional(command.Email, "email", MaxEmailLength);
        if (Email is not null && (!Email.Contains('@') || Email.StartsWith('@') || Email.EndsWith('@')))
            throw new ArgumentException("Email is not valid.", "email");
        Phone = Optional(command.Phone, "phone", MaxPhoneLength);
        Address = Optional(command.Address, "address", MaxAddressLength);
    }

    public void SetWarehouse(double latitude, double longitude, double radiusKm)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
        if (latitude == 0 && longitude == 0)
            throw new ArgumentException("(0,0) is not a valid warehouse location.");
        if (radiusKm is <= 0 or > MaxWarehouseRadiusKm)
            throw new ArgumentException($"Radius must be greater than 0 and at most {MaxWarehouseRadiusKm} km.", nameof(radiusKm));

        WarehouseLatitude = latitude;
        WarehouseLongitude = longitude;
        WarehouseRadiusKm = radiusKm;
    }

    private static string? Optional(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{field} cannot exceed {maxLength} characters.", field);
        return trimmed;
    }
}
