using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Profiles.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST.Transform;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;

namespace RebooTech.Fruitlogix.Platform.Profiles.Interfaces.REST;

/// <summary>
///     Perfil del cliente comercial autenticado (razón social, RUC, contacto y almacén).
///     Solo para el rol CommercialClient; el perfil del productor está en /producers.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class ProfilesController(
    ICommercialClientCommandService commandService,
    ICommercialClientQueryService queryService) : ControllerBase
{
    /// <summary>Perfil del cliente autenticado. 404 si todavía no lo completó.</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(CommercialClientProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!me.IsCommercialClient) return OnlyClients();

        var client = await queryService.Handle(new GetCommercialClientByUserIdQuery(me.Id));
        if (client is null) return NotFound(new { message = "Profile not created yet. Use PUT /api/v1/profiles/me." });
        return Ok(CommercialClientProfileResourceAssembler.ToResourceFromEntity(client));
    }

    /// <summary>Crea o reemplaza el perfil del cliente autenticado.</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(CommercialClientProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateCommercialClientProfileResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!me.IsCommercialClient) return OnlyClients();

        // Valida el almacén antes de guardar nada, para no dejar el perfil a medio actualizar.
        if (resource.Warehouse is { } warehouse)
            new CommercialClient(me.Id).SetWarehouse(warehouse.Latitude, warehouse.Longitude, warehouse.RadiusKm);

        var client = await commandService.Handle(
            CommercialClientProfileResourceAssembler.ToCommandFromResource(me.Id, resource));
        if (resource.Warehouse is not null)
            client = await commandService.Handle(
                CommercialClientProfileResourceAssembler.ToCommandFromResource(me.Id, resource.Warehouse));
        return Ok(CommercialClientProfileResourceAssembler.ToResourceFromEntity(client));
    }

    /// <summary>
    ///     Fija la geocerca del almacén. Cuando un tracking log de una de sus entregas cae dentro del radio,
    ///     la entrega se marca como Arrived automáticamente.
    /// </summary>
    [HttpPut("me/warehouse")]
    [ProducesResponseType(typeof(CommercialClientProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetMyWarehouse([FromBody] WarehouseLocationResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (!me.IsCommercialClient) return OnlyClients();

        var client = await commandService.Handle(
            CommercialClientProfileResourceAssembler.ToCommandFromResource(me.Id, resource));
        return Ok(CommercialClientProfileResourceAssembler.ToResourceFromEntity(client));
    }

    private ObjectResult OnlyClients() =>
        StatusCode(StatusCodes.Status403Forbidden,
            new { message = "Only commercial clients have this profile. Producers use /api/v1/producers." });
}
