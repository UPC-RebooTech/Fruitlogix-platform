using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;

public interface IInvoiceQueryService
{
    Task<IEnumerable<Invoice>> Handle(GetAllInvoicesQuery query);
    Task<Invoice?> Handle(GetInvoiceByIdQuery query);
}