using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;

/// <summary>
///     Revisa si un pedido tiene registros de otros contextos (entrega, factura, pago)
///     que impiden borrarlo físicamente.
/// </summary>
public interface IOrderDependencyService
{
    /// <summary>Nombres de los tipos de registro asociados ("delivery", "invoice", "payment"); vacío si no hay.</summary>
    Task<IReadOnlyList<string>> GetDependenciesAsync(int orderId);
}

public class OrderDependencyService(AppDbContext context) : IOrderDependencyService
{
    public async Task<IReadOnlyList<string>> GetDependenciesAsync(int orderId)
    {
        var dependencies = new List<string>();

        if (await context.Set<Delivery>().AnyAsync(d => d.OrderId == orderId))
            dependencies.Add("delivery");

        var invoiceIds = await context.Set<Invoice>()
            .Where(i => i.OrderId == orderId)
            .Select(i => i.Id)
            .ToListAsync();

        if (invoiceIds.Count > 0)
        {
            dependencies.Add("invoice");
            if (await context.Set<PaymentTransaction>().AnyAsync(t => invoiceIds.Contains(t.InvoiceId)))
                dependencies.Add("payment");
        }

        return dependencies;
    }
}
