using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.QueryServices;

public class InvoiceQueryService(IInvoiceRepository invoiceRepository) : IInvoiceQueryService
{
    public async Task<IEnumerable<Invoice>> Handle(GetAllInvoicesQuery query) =>
        await invoiceRepository.ListAsync();

    public async Task<Invoice?> Handle(GetInvoiceByIdQuery query) =>  // ← NUEVO
        await invoiceRepository.FindByIdAsync(query.Id);
}