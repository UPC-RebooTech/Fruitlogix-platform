namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Queries;

/// <param name="ProducerId">Solo productos de ese productor.</param>
/// <param name="FruitType">Tipo de fruta (sin distinguir mayúsculas).</param>
/// <param name="Available">true = activos con stock; false = sin stock o inactivos.</param>
/// <param name="IncludeInactive">false oculta los productos desactivados.</param>
public record GetProductsQuery(int? ProducerId, string? FruitType, bool? Available, bool IncludeInactive);

public record GetProductByIdQuery(int ProductId);
