using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest;
using ProducerAggregate = RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates.Producer;

namespace RebooTech.Fruitlogix.Platform.QualityControl.Interfaces.REST;

/// <summary>
///     Cosechas programadas.
///     <list type="bullet">
///         <item>Producer: ve y gestiona solo las suyas (el producerId sale de su perfil).</item>
///         <item>Distributor y Admin: ven y gestionan todas (filtro opcional ?producerId=).</item>
///         <item>CommercialClient: la lista viene vacía y las escrituras responden 403.</item>
///     </list>
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[DomainExceptionFilter]
public class HarvestPlansController(
    IHarvestPlanCommandService commandService,
    IHarvestPlanQueryService queryService,
    IProducerQueryService producerQueryService,
    IAccessScopeService accessScope) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HarvestPlanResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHarvestPlans([FromQuery] int? producerId)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return Ok(Array.Empty<HarvestPlanResource>());

        if (me.IsProducer)
        {
            var ownProducerId = await accessScope.GetProducerIdAsync(me);
            if (ownProducerId is null || (producerId is not null && producerId != ownProducerId))
                return Ok(Array.Empty<HarvestPlanResource>());
            producerId = ownProducerId;
        }

        var plans = (await queryService.Handle(new GetHarvestPlansQuery(producerId))).ToList();
        return Ok(await ToResourcesAsync(plans));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HarvestPlanResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHarvestPlanById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();

        var plan = await queryService.Handle(new GetHarvestPlanByIdQuery(id));
        if (plan is null || !await CanManageAsync(me, plan)) return NotFoundPlan(id);
        return Ok((await ToResourcesAsync([plan]))[0]);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HarvestPlanResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateHarvestPlan([FromBody] SaveHarvestPlanResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return ForbiddenWrite();

        var (producerId, error) = await ResolveProducerIdAsync(me, resource.ProducerId);
        if (error is not null) return error;

        var plan = await commandService.Handle(ToCommand(resource, producerId!.Value));
        var result = (await ToResourcesAsync([plan]))[0];
        return CreatedAtAction(nameof(GetHarvestPlanById), new { id = result.Id }, result);
    }

    /// <summary>Reemplaza la cosecha programada.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(HarvestPlanResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateHarvestPlan(int id, [FromBody] SaveHarvestPlanResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return ForbiddenWrite();

        var existing = await queryService.Handle(new GetHarvestPlanByIdQuery(id));
        if (existing is null || !await CanManageAsync(me, existing)) return NotFoundPlan(id);

        // Admin/Distributor pueden reasignar el productor; si no envían producerId se conserva.
        var (producerId, error) = await ResolveProducerIdAsync(me, resource.ProducerId ?? existing.ProducerId);
        if (error is not null) return error;

        var plan = await commandService.Handle(new UpdateHarvestPlanCommand(id, ToCommand(resource, producerId!.Value)));
        if (plan is null) return NotFoundPlan(id);
        return Ok((await ToResourcesAsync([plan]))[0]);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHarvestPlan(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        if (me.IsCommercialClient) return ForbiddenWrite();

        var existing = await queryService.Handle(new GetHarvestPlanByIdQuery(id));
        if (existing is null || !await CanManageAsync(me, existing)) return NotFoundPlan(id);

        return await commandService.Handle(new DeleteHarvestPlanCommand(id)) ? NoContent() : NotFoundPlan(id);
    }

    /// <summary>
    ///     Producer: siempre su perfil (409 si no tiene). Admin/Distributor: el producerId pedido, que debe existir (400).
    /// </summary>
    private async Task<(int? ProducerId, IActionResult? Error)> ResolveProducerIdAsync(CurrentUser me, int? requested)
    {
        if (me.IsProducer)
        {
            var own = await accessScope.GetProducerIdAsync(me);
            return own is null
                ? (null, Conflict(new { message = "Create your producer profile first (POST /api/v1/producers)." }))
                : (own, null);
        }

        if (requested is not { } producerId)
            return (null, BadRequest(new { message = "producerId is required." }));
        if (await producerQueryService.Handle(new GetProducerByIdQuery(producerId)) is null)
            return (null, BadRequest(new { message = $"Producer {producerId} not found." }));
        return (producerId, null);
    }

    /// <summary>Admin/Distributor: cualquiera. Producer: solo las suyas. Cliente: ninguna.</summary>
    private async Task<bool> CanManageAsync(CurrentUser me, HarvestPlan plan)
    {
        if (me.IsAdmin || me.IsDistributor) return true;
        return me.IsProducer && await accessScope.GetProducerIdAsync(me) == plan.ProducerId;
    }

    private static SaveHarvestPlanCommand ToCommand(SaveHarvestPlanResource r, int producerId) =>
        new(producerId, r.FruitType, r.ScheduledFor, r.EstimatedYieldKg, r.Zone, r.CrewName, r.Pickers, r.Notes);

    private ObjectResult ForbiddenWrite() =>
        StatusCode(StatusCodes.Status403Forbidden,
            new { message = "Only producers, distributors or admins can manage harvest plans." });

    private NotFoundObjectResult NotFoundPlan(int id) =>
        NotFound(new { message = $"Harvest plan {id} not found." });

    private static string ProducerDisplayName(ProducerAggregate producer) =>
        string.IsNullOrWhiteSpace(producer.FullName) ? producer.LegalName : producer.FullName;

    private async Task<List<HarvestPlanResource>> ToResourcesAsync(IReadOnlyCollection<HarvestPlan> plans)
    {
        if (plans.Count == 0) return [];
        var names = (await producerQueryService.Handle(new GetAllProducersQuery()))
            .ToDictionary(p => p.Id, ProducerDisplayName);
        return plans.Select(p => new HarvestPlanResource(
            p.Id, p.ProducerId, names.GetValueOrDefault(p.ProducerId), p.FruitType, p.ScheduledFor,
            p.EstimatedYieldKg, p.Zone, p.CrewName, p.Pickers, p.Notes, p.CreatedAt, p.UpdatedAt)).ToList();
    }
}
