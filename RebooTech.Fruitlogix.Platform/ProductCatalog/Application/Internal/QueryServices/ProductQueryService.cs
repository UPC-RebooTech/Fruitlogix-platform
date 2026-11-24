using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Application.Internal.QueryServices;

public class ProductQueryService(IProductRepository productRepository) : IProductQueryService
{
    public Task<IEnumerable<Product>> Handle(GetProductsQuery query) => productRepository.SearchAsync(query);

    public Task<Product?> Handle(GetProductByIdQuery query) => productRepository.FindByIdAsync(query.ProductId);
}
