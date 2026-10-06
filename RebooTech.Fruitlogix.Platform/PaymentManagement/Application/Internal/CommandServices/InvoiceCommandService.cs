using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.CommandServices;

public class InvoiceCommandService(
    IInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork) : IInvoiceCommandService
{
    public async Task<Invoice?> Handle(CreateInvoiceCommand command)
    {
        var invoice = new Invoice(command);
        await invoiceRepository.AddAsync(invoice);
        await unitOfWork.CompleteAsync();
        return invoice;
    }
}