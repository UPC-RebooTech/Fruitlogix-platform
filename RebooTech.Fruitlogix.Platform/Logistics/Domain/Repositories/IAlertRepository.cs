using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;

public interface IAlertRepository : IBaseRepository<Alert>
{
    Task<IEnumerable<Alert>> FindActiveAlertsAsync();
}