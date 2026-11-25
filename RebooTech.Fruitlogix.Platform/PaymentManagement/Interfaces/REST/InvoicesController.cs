using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Naming;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController(
    IInvoiceCommandService invoiceCommandService,
    IInvoiceQueryService invoiceQueryService,
    IAccessScopeService accessScope,
    IClientNameService clientNameService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceResource resource)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var command = CreateInvoiceCommandFromResourceAssembler.ToCommandFromResource(resource)
            with { IssuedByUserId = me.Id };
        var invoice = await invoiceCommandService.Handle(command);
        if (invoice is null) return BadRequest();
        var invoiceResource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice,
            await clientNameService.GetNameAsync(invoice.ClientId));
        return StatusCode(201, invoiceResource);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InvoiceResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllInvoices()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query = new GetAllInvoicesQuery();
        var invoices = await invoiceQueryService.Handle(query);
        var visible = await accessScope.GetVisibleInvoiceIdsAsync(me);
        var visibleInvoices = invoices.WhereVisible(visible, i => i.Id).ToList();
        var clientNames = await clientNameService.GetNamesAsync(visibleInvoices.Select(i => i.ClientId));
        var resources = visibleInvoices.Select(i =>
            InvoiceResourceFromEntityAssembler.ToResourceFromEntity(i, clientNames.GetValueOrDefault(i.ClientId)));
        return Ok(resources);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InvoiceResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoiceById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query = new GetInvoiceByIdQuery(id);
        var invoice = await invoiceQueryService.Handle(query);
        if (invoice is null) return NotFound();
        var visible = await accessScope.GetVisibleInvoiceIdsAsync(me);
        if (visible is not null && !visible.Contains(invoice.Id)) return NotFound();
        return Ok(InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice,
            await clientNameService.GetNameAsync(invoice.ClientId)));
    }
}