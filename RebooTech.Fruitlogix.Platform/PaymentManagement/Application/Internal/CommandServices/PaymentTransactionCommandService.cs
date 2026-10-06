using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.CommandServices;

public class PaymentTransactionCommandService(
    IPaymentTransactionRepository transactionRepository,
    IInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork) : IPaymentTransactionCommandService
{
    public async Task<PaymentTransaction?> Handle(CreatePaymentTransactionCommand command)
    {
        var invoice = await invoiceRepository.FindByIdAsync(command.InvoiceId);
        if (invoice is null) return null;

        var transaction = new PaymentTransaction(command);
        await transactionRepository.AddAsync(transaction);

        transaction.MarkAsSuccess();
        invoice.MarkAsPaid();

        await unitOfWork.CompleteAsync();
        return transaction;
    }
}