using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;

/// <summary>
///     Calcula qué datos puede ver el usuario autenticado según su rol.
///     Los métodos devuelven <c>null</c> cuando el usuario ve todo (Admin y Distributor)
///     y un conjunto de ids (posiblemente vacío) cuando hay que filtrar.
/// </summary>
public interface IAccessScopeService
{
    /// <summary>Id del perfil de productor enlazado al usuario (producers.user_id), o null.</summary>
    Task<int?> GetProducerIdAsync(CurrentUser user);

    Task<IReadOnlySet<int>?> GetVisibleOrderIdsAsync(CurrentUser user);
    Task<IReadOnlySet<int>?> GetVisibleDeliveryIdsAsync(CurrentUser user);
    Task<IReadOnlySet<int>?> GetVisibleInvoiceIdsAsync(CurrentUser user);
    Task<Func<Incident, bool>?> GetIncidentFilterAsync(CurrentUser user);
}

public class AccessScopeService(AppDbContext context) : IAccessScopeService
{
    private static readonly IReadOnlySet<int> Empty = new HashSet<int>();

    private static bool SeesAll(CurrentUser user) => user.IsAdmin || user.IsDistributor;

    public async Task<int?> GetProducerIdAsync(CurrentUser user)
    {
        if (!user.IsProducer) return null;
        return await context.Set<Producer>()
            .Where(p => p.UserId == user.Id)
            .OrderBy(p => p.Id)
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlySet<int>?> GetVisibleOrderIdsAsync(CurrentUser user)
    {
        if (SeesAll(user)) return null;

        if (user.IsCommercialClient)
            return (await context.Set<Order>()
                .Where(o => o.CommercialClientId == user.Id)
                .Select(o => o.Id)
                .ToListAsync()).ToHashSet();

        if (user.IsProducer)
        {
            var producerId = await GetProducerIdAsync(user);
            if (producerId is null) return Empty;
            // ProducerId es un value object con conversión: se filtra en memoria,
            // igual que OrderRepository.FindByProducerIdAsync.
            var orders = await context.Set<Order>()
                .Select(o => new { o.Id, o.ProducerId })
                .ToListAsync();
            return orders
                .Where(o => o.ProducerId != null && o.ProducerId.Value == producerId)
                .Select(o => o.Id)
                .ToHashSet();
        }

        return Empty;
    }

    public async Task<IReadOnlySet<int>?> GetVisibleDeliveryIdsAsync(CurrentUser user)
    {
        var orderIds = await GetVisibleOrderIdsAsync(user);
        if (orderIds is null) return null;
        if (orderIds.Count == 0) return Empty;

        var ids = orderIds.ToList();
        return (await context.Set<Delivery>()
            .Where(d => ids.Contains(d.OrderId))
            .Select(d => d.Id)
            .ToListAsync()).ToHashSet();
    }

    public async Task<IReadOnlySet<int>?> GetVisibleInvoiceIdsAsync(CurrentUser user)
    {
        if (SeesAll(user)) return null;
        if (!user.IsCommercialClient) return Empty;

        return (await context.Set<Invoice>()
            .Where(i => i.ClientId == user.Id)
            .Select(i => i.Id)
            .ToListAsync()).ToHashSet();
    }

    public async Task<Func<Incident, bool>?> GetIncidentFilterAsync(CurrentUser user)
    {
        if (SeesAll(user)) return null;

        var orderIds = await GetVisibleOrderIdsAsync(user) ?? Empty;
        IReadOnlySet<int> batchIds = Empty;

        if (user.IsProducer)
        {
            var producerId = await GetProducerIdAsync(user);
            if (producerId is not null)
                batchIds = (await context.Set<HarvestBatch>()
                    .Where(b => b.ProducerId == producerId.Value)
                    .Select(b => b.Id)
                    .ToListAsync()).ToHashSet();
        }

        var userId = user.Id;
        return incident =>
            incident.ReportedByUserId == userId
            || (incident.OrderId is { } orderId && orderIds.Contains(orderId))
            || batchIds.Contains(incident.BatchId);
    }
}

public static class AccessScopeEnumerableExtensions
{
    /// <summary>Filtra por ids visibles; si <paramref name="visibleIds" /> es null (ve todo) no filtra.</summary>
    public static IEnumerable<T> WhereVisible<T>(this IEnumerable<T> source, IReadOnlySet<int>? visibleIds,
        Func<T, int> idSelector) =>
        visibleIds is null ? source : source.Where(item => visibleIds.Contains(idSelector(item)));
}
