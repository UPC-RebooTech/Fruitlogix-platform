using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.QueryServices;

public interface ISensorReadingQueryService
{
    Task<IEnumerable<SensorReading>> Handle(GetAllSensorReadingsQuery query);
    Task<IEnumerable<SensorReading>> Handle(GetSensorReadingsByDeviceIdQuery query); // ← nueva firma
}