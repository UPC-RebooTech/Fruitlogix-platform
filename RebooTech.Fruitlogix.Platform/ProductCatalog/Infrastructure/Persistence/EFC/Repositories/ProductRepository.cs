using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Infrastructure.Persistence.EFC.Repositories;

public class ProductRepository(AppDbContext context)
    : BaseRepository<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> SearchAsync(GetProductsQuery query)
    {
        var products = Context.Set<Product>().AsQueryable();

        if (!query.IncludeInactive)
            products = products.Where(p => p.IsActive);

        if (query.ProducerId is { } producerId)
            products = products.Where(p => p.ProducerId == producerId);

        if (!string.IsNullOrWhiteSpace(query.FruitType))
        {
            // La collation de MySQL no distingue mayúsculas; ToLower lo garantiza igual.
            var fruitType = query.FruitType.Trim().ToLower();
            products = products.Where(p => p.FruitType.ToLower() == fruitType);
        }

        if (query.Available is { } available)
            products = available
                ? products.Where(p => p.IsActive && p.AvailableKg > 0)
                : products.Where(p => !p.IsActive || p.AvailableKg <= 0);

        return await products.OrderBy(p => p.Name).ThenBy(p => p.Id).ToListAsync();
    }
}
