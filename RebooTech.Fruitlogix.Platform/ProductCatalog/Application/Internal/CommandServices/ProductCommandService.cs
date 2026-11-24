using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Application.Internal.CommandServices;

public class ProductCommandService(IProductRepository productRepository, IUnitOfWork unitOfWork)
    : IProductCommandService
{
    public async Task<Product> Handle(CreateProductCommand command)
    {
        var product = new Product(command);
        await productRepository.AddAsync(product);
        await unitOfWork.CompleteAsync();
        return product;
    }

    public async Task<Product?> Handle(UpdateProductCommand command)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId);
        if (product is null) return null;
        product.Update(command);
        productRepository.Update(product);
        await unitOfWork.CompleteAsync();
        return product;
    }

    public async Task<bool> Handle(DeactivateProductCommand command)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId);
        if (product is null) return false;
        product.Deactivate();
        productRepository.Update(product);
        await unitOfWork.CompleteAsync();
        return true;
    }
}
