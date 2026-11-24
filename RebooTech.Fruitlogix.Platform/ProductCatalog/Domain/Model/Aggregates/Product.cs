using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;

/// <summary>
///     Producto del catálogo de un productor (precio por kg y stock disponible).
///     Borrar un producto lo desactiva (<see cref="IsActive" /> = false) para no romper
///     los ítems de pedidos que ya lo referencian.
/// </summary>
public class Product : IAuditableEntity
{
    public const int MaxNameLength = 100;
    public const int MaxFruitTypeLength = 100;
    public const int MaxVarietyLength = 100;
    public const int MaxDescriptionLength = 1000;
    public const int MaxImageUrlLength = 500;

    public int Id { get; private set; }
    /// <summary>Id del perfil en /producers.</summary>
    public int ProducerId { get; private set; }
    public string Name { get; private set; } = null!;
    public string FruitType { get; private set; } = null!;
    public string? Variety { get; private set; }
    public string? Description { get; private set; }
    /// <summary>Precio por kg.</summary>
    public decimal UnitPrice { get; private set; }
    public double AvailableKg { get; private set; }
    public double MinOrderKg { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>Activo y con stock. No se guarda en la base de datos.</summary>
    public bool IsAvailable => IsActive && AvailableKg > 0;

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Product() { }

    public Product(CreateProductCommand command)
    {
        if (command.ProducerId <= 0)
            throw new ArgumentException("ProducerId must be a positive integer.");
        ProducerId = command.ProducerId;
        SetDetails(command.Name, command.FruitType, command.Variety, command.Description,
            command.UnitPrice, command.AvailableKg, command.MinOrderKg, command.ImageUrl);
        IsActive = true;
    }

    public void Update(UpdateProductCommand command)
    {
        SetDetails(command.Name, command.FruitType, command.Variety, command.Description,
            command.UnitPrice, command.AvailableKg, command.MinOrderKg, command.ImageUrl);
        IsActive = command.IsActive;
    }

    public void Deactivate() => IsActive = false;

    private void SetDetails(string name, string fruitType, string? variety, string? description,
        decimal unitPrice, double availableKg, double minOrderKg, string? imageUrl)
    {
        Name = Required(name, nameof(name), MaxNameLength);
        FruitType = Required(fruitType, nameof(fruitType), MaxFruitTypeLength);
        Variety = Optional(variety, nameof(variety), MaxVarietyLength);
        Description = Optional(description, nameof(description), MaxDescriptionLength);
        ImageUrl = Optional(imageUrl, nameof(imageUrl), MaxImageUrlLength);

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));
        if (availableKg < 0)
            throw new ArgumentException("Available kg cannot be negative.", nameof(availableKg));
        if (minOrderKg < 0)
            throw new ArgumentException("Minimum order kg cannot be negative.", nameof(minOrderKg));

        UnitPrice = Math.Round(unitPrice, 2, MidpointRounding.AwayFromZero);
        AvailableKg = availableKg;
        MinOrderKg = minOrderKg;
    }

    private static string Required(string? value, string field, int maxLength) =>
        Optional(value, field, maxLength) ?? throw new ArgumentException($"{field} is required.", field);

    private static string? Optional(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{field} cannot exceed {maxLength} characters.", field);
        return trimmed;
    }
}
