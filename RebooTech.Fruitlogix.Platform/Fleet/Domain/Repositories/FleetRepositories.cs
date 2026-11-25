using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Fleet.Domain.Repositories;

public interface IDriverRepository : IBaseRepository<Driver>
{
}

public interface IVehicleRepository : IBaseRepository<Vehicle>
{
    /// <summary>true si otra unidad (distinta de excludeVehicleId) ya usa esa placa.</summary>
    Task<bool> ExistsByPlateAsync(string plate, int? excludeVehicleId = null);
}
