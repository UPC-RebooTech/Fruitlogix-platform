using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Application.CommandServices;

public interface IProductCommandService
{
    Task<Product> Handle(CreateProductCommand command);
    Task<Product?> Handle(UpdateProductCommand command);
    /// <summary>false si el producto no existe.</summary>
    Task<bool> Handle(DeactivateProductCommand command);
}
