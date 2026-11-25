using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.Fleet.Infrastructure.Persistence.EFC.Repositories;

public class DriverRepository(AppDbContext context) : BaseRepository<Driver>(context), IDriverRepository
{
}

public class VehicleRepository(AppDbContext context) : BaseRepository<Vehicle>(context), IVehicleRepository
{
    public Task<bool> ExistsByPlateAsync(string plate, int? excludeVehicleId = null) =>
        Context.Set<Vehicle>().AnyAsync(v => v.Plate == plate && (excludeVehicleId == null || v.Id != excludeVehicleId));
}
