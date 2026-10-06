using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;

public interface IPaymentTransactionQueryService
{
    Task<IEnumerable<PaymentTransaction>> Handle(GetAllPaymentTransactionsQuery query);
    Task<PaymentTransaction?> Handle(GetPaymentTransactionByIdQuery query);
}