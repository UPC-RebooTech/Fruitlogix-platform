namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Commands;

public record CreateProductCommand(
    int ProducerId,
    string Name,
    string FruitType,
    string? Variety,
    string? Description,
    decimal UnitPrice,
    double AvailableKg,
    double MinOrderKg,
    string? ImageUrl
);

public record UpdateProductCommand(
    int ProductId,
    string Name,
    string FruitType,
    string? Variety,
    string? Description,
    decimal UnitPrice,
    double AvailableKg,
    double MinOrderKg,
    string? ImageUrl,
    bool IsActive
);

public record DeactivateProductCommand(int ProductId);
