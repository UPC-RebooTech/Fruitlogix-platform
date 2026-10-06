using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Infrastructure.Persistence.EFC.Repositories;

public class SensorReadingRepository(AppDbContext context)
    : BaseRepository<SensorReading>(context), ISensorReadingRepository
{
    public async Task<IEnumerable<SensorReading>> FindByDeviceIdAsync(int deviceId)
        => await context.Set<SensorReading>()
            .Where(r => r.DeviceId == deviceId)
            .ToListAsync();
}