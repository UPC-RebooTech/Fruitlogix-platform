using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;
using ProducerAggregate = RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates.Producer;

namespace RebooTech.Fruitlogix.Platform.ProductCatalog.Interfaces.REST;

/// <summary>
///     Catálogo de productos.
///     <list type="bullet">
///         <item>CommercialClient: ve los productos activos de todos los productores.</item>
///         <item>Producer: ve y administra solo los suyos (incluidos los inactivos).</item>
///         <item>Admin y Distributor: ven y administran todos.</item>
///     </list>
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class ProductsController(
    IProductCommandService productCommandService,
    IProductQueryService productQueryService,
    IProducerQueryService producerQueryService,
    IAccessScopeService accessScope) : ControllerBase
{
    /// <summary>Lista productos. Filtros opcionales: producerId, fruitType y available.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int? producerId, [FromQuery] string? fruitType, [FromQuery] bool? available)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();

        if (me.IsProducer)
        {
            var ownProducerId = await accessScope.GetProducerIdAsync(me);
            if (ownProducerId is null || (producerId is not null && producerId != ownProducerId))
                return Ok(Array.Empty<ProductResource>());
            producerId = ownProducerId;
        }

        var products = await productQueryService.Handle(
            new GetProductsQuery(producerId, fruitType, available, IncludeInactive: !me.IsCommercialClient));
        return Ok(await ToResourcesAsync(products));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();

        var product = await productQueryService.Handle(new GetProductByIdQuery(id));
        if (product is null || !await CanReadAsync(me, product)) return NotFound();
        return Ok(await ToResourceAsync(product));
    }

    /// <summary>Crea un producto. Un Producer lo crea en su propio perfil; Admin y Distributor indican producerId.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return ForbiddenWrite();

        int producerId;
        if (me.IsProducer)
        {
            if (await accessScope.GetProducerIdAsync(me) is not { } ownProducerId)
                return Conflict(new { message = "Create your producer profile first (POST /api/v1/producers)." });
            producerId = ownProducerId;
        }
        else
        {
            if (resource.ProducerId is not { } requestedProducerId)
                return BadRequest(new { message = "ProducerId is required." });
            if (await producerQueryService.Handle(new GetProducerByIdQuery(requestedProducerId)) is null)
                return BadRequest(new { message = $"Producer {requestedProducerId} not found." });
            producerId = requestedProducerId;
        }

        var product = await productCommandService.Handle(
            ProductResourceAssembler.ToCommandFromResource(resource, producerId));
        var result = await ToResourceAsync(product);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return ForbiddenWrite();

        var product = await productQueryService.Handle(new GetProductByIdQuery(id));
        if (product is null || !await CanManageAsync(me, product)) return NotFound();

        var updated = await productCommandService.Handle(ProductResourceAssembler.ToCommandFromResource(id, resource));
        if (updated is null) return NotFound();
        return Ok(await ToResourceAsync(updated));
    }

    /// <summary>Desactiva el producto (no se borra: los pedidos existentes lo siguen referenciando).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return ForbiddenWrite();

        var product = await productQueryService.Handle(new GetProductByIdQuery(id));
        if (product is null || !await CanManageAsync(me, product)) return NotFound();

        return await productCommandService.Handle(new DeactivateProductCommand(id)) ? NoContent() : NotFound();
    }

    private ObjectResult ForbiddenWrite() =>
        StatusCode(StatusCodes.Status403Forbidden,
            new { message = "Only producers, distributors or admins can manage products." });

    /// <summary>Admin/Distributor: todo. Producer: solo los suyos. Cliente: solo activos.</summary>
    private async Task<bool> CanReadAsync(CurrentUser me, Product product)
    {
        if (me.IsCommercialClient) return product.IsActive;
        return await CanManageAsync(me, product);
    }

    private async Task<bool> CanManageAsync(CurrentUser me, Product product)
    {
        if (me.IsAdmin || me.IsDistributor) return true;
        if (!me.IsProducer) return false;
        return await accessScope.GetProducerIdAsync(me) == product.ProducerId;
    }

    private static string ProducerDisplayName(ProducerAggregate producer) =>
        string.IsNullOrWhiteSpace(producer.FullName) ? producer.LegalName : producer.FullName;

    private async Task<ProductResource> ToResourceAsync(Product product)
    {
        var producer = await producerQueryService.Handle(new GetProducerByIdQuery(product.ProducerId));
        return ProductResourceAssembler.ToResourceFromEntity(product,
            producer is null ? null : ProducerDisplayName(producer));
    }

    private async Task<List<ProductResource>> ToResourcesAsync(IEnumerable<Product> products)
    {
        var list = products.ToList();
        if (list.Count == 0) return [];
        var names = (await producerQueryService.Handle(new GetAllProducersQuery()))
            .ToDictionary(p => p.Id, ProducerDisplayName);
        return list.Select(p => ProductResourceAssembler.ToResourceFromEntity(p,
            names.TryGetValue(p.ProducerId, out var name) ? name : null)).ToList();
    }
}
