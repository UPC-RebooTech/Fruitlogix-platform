using Microsoft.AspNetCore.Mvc;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Resources;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST.Transform;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/payment-transactions")]
public class PaymentTransactionsController(
    IPaymentTransactionCommandService paymentTransactionCommandService,
    IPaymentTransactionQueryService paymentTransactionQueryService,
    IAccessScopeService accessScope) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PaymentTransactionResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePaymentTransaction(
        [FromBody] CreatePaymentTransactionResource resource)
    {
        var command = CreatePaymentTransactionCommandFromResourceAssembler
            .ToCommandFromResource(resource);
        var transaction = await paymentTransactionCommandService.Handle(command);
        if (transaction is null) return NotFound();
        var transactionResource =
            PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity(transaction);
        return StatusCode(201, transactionResource);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentTransactionResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPaymentTransactions()
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query = new GetAllPaymentTransactionsQuery();
        var transactions = await paymentTransactionQueryService.Handle(query);
        var visibleInvoices = await accessScope.GetVisibleInvoiceIdsAsync(me);
        var resources = transactions
            .WhereVisible(visibleInvoices, t => t.InvoiceId)
            .Select(PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentTransactionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentTransactionById(int id)
    {
        if (this.GetCurrentUser() is not { } me) return Unauthorized();
        var query = new GetPaymentTransactionByIdQuery(id);
        var transaction = await paymentTransactionQueryService.Handle(query);
        if (transaction is null) return NotFound();
        var visibleInvoices = await accessScope.GetVisibleInvoiceIdsAsync(me);
        if (visibleInvoices is not null && !visibleInvoices.Contains(transaction.InvoiceId)) return NotFound();
        return Ok(PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity(transaction));
    }
    
}