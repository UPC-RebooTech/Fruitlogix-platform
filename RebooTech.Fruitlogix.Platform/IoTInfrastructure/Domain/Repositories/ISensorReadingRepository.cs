using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;

public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    Task<IEnumerable<SensorReading>> FindByDeviceIdAsync(int deviceId);
}