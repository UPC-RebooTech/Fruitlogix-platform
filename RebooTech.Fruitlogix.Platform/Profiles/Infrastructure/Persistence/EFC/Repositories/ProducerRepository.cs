using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class ProducerRepository(AppDbContext context)
    : BaseRepository<Producer>(context), IProducerRepository
{
    public async Task<bool> ExistsByTaxIdAsync(string taxId) =>
        await Context.Set<Producer>()
            .AnyAsync(p => p.TaxId.Value == taxId);
    public async Task<Producer?> FindByTaxIdAsync(string taxId) =>
        await Context.Set<Producer>()
            .FirstOrDefaultAsync(p => p.TaxId.Value == taxId);
}