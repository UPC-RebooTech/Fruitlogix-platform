using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;

public interface IProducerRepository : IBaseRepository<Producer>
{
    Task<bool> ExistsByTaxIdAsync(string taxId);
    
    Task<Producer?> FindByTaxIdAsync(string taxId);
}