using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Application.QueryServices;

public interface IProductQueryService
{
    Task<IEnumerable<Product>> Handle(GetProductsQuery query);
    Task<Product?> Handle(GetProductByIdQuery query);
}
