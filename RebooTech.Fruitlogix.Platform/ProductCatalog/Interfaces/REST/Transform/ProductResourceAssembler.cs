using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Interfaces.REST.Resources;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Interfaces.REST.Transform;

public static class ProductResourceAssembler
{
    public static ProductResource ToResourceFromEntity(Product product, string? producerName = null) =>
        new(
            product.Id,
            product.ProducerId,
            producerName,
            product.Name,
            product.FruitType,
            product.Variety,
            product.Description,
            product.UnitPrice,
            product.AvailableKg,
            product.MinOrderKg,
            product.ImageUrl,
            product.IsActive,
            product.IsAvailable,
            product.CreatedAt,
            product.UpdatedAt
        );

    public static CreateProductCommand ToCommandFromResource(CreateProductResource resource, int producerId) =>
        new(
            producerId,
            resource.Name,
            resource.FruitType,
            resource.Variety,
            resource.Description,
            resource.UnitPrice,
            resource.AvailableKg,
            resource.MinOrderKg,
            resource.ImageUrl
        );

    public static UpdateProductCommand ToCommandFromResource(int productId, UpdateProductResource resource) =>
        new(
            productId,
            resource.Name,
            resource.FruitType,
            resource.Variety,
            resource.Description,
            resource.UnitPrice,
            resource.AvailableKg,
            resource.MinOrderKg,
            resource.ImageUrl,
            resource.IsActive
        );
}
