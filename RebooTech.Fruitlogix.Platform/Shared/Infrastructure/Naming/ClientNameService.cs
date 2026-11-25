using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Naming;

/// <summary>
///     Nombre visible del cliente comercial: razón social de su perfil (/profiles/me) o, si no la tiene,
///     su username. Lo usan pedidos y facturas para que el productor no necesite consultar /users.
/// </summary>
public interface IClientNameService
{
    Task<string?> GetNameAsync(int clientUserId);
    Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IEnumerable<int> clientUserIds);
}

public class ClientNameService(AppDbContext context) : IClientNameService
{
    public async Task<string?> GetNameAsync(int clientUserId) =>
        (await GetNamesAsync([clientUserId])).GetValueOrDefault(clientUserId);

    public async Task<IReadOnlyDictionary<int, string>> GetNamesAsync(IEnumerable<int> clientUserIds)
    {
        var ids = clientUserIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<int, string>();

        var usernames = await context.Set<User>().AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.Username })
            .ToListAsync();
        var businessNames = await context.Set<CommercialClient>().AsNoTracking()
            .Where(c => ids.Contains(c.UserId) && c.BusinessName != null)
            .Select(c => new { c.UserId, c.BusinessName })
            .ToListAsync();

        var names = usernames.ToDictionary(u => u.Id, u => u.Username);
        foreach (var client in businessNames)
            names[client.UserId] = client.BusinessName!;
        return names;
    }
}
