using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.PaymentManagement.Infrastructure.Persistence.EFC.Repositories;

public class InvoiceRepository(AppDbContext context)
    : BaseRepository<Invoice>(context), IInvoiceRepository
{
}