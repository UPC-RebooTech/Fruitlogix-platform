namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Interfaces.REST.Resources;

public record ProductResource(
    int Id,
    int ProducerId,
    string? ProducerName,
    string Name,
    string FruitType,
    string? Variety,
    string? Description,
    decimal UnitPrice,
    double AvailableKg,
    double MinOrderKg,
    string? ImageUrl,
    bool IsActive,
    bool IsAvailable,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

/// <param name="ProducerId">Solo lo usan Admin y Distributor; para un Producer se toma su propio perfil.</param>
/// <param name="UnitPrice">Precio por kg.</param>
public record CreateProductResource(
    string Name,
    string FruitType,
    decimal UnitPrice,
    double AvailableKg,
    string? Variety = null,
    string? Description = null,
    double MinOrderKg = 0,
    string? ImageUrl = null,
    int? ProducerId = null
);

public record UpdateProductResource(
    string Name,
    string FruitType,
    decimal UnitPrice,
    double AvailableKg,
    string? Variety = null,
    string? Description = null,
    double MinOrderKg = 0,
    string? ImageUrl = null,
    bool IsActive = true
);
