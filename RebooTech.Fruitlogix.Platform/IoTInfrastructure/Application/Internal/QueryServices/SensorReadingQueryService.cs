using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.Internal.QueryServices;

public class SensorReadingQueryService(ISensorReadingRepository repository)
    : ISensorReadingQueryService
{
    public async Task<IEnumerable<SensorReading>> Handle(GetAllSensorReadingsQuery query)
        => await repository.ListAsync();

    public async Task<IEnumerable<SensorReading>> Handle(GetSensorReadingsByDeviceIdQuery query) // ← nueva impl
        => await repository.FindByDeviceIdAsync(query.DeviceId);
}