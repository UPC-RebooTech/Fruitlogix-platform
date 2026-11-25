using RebooTech.Fruitlogix.Platform.Fleet.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Queries;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Fleet.Application.Internal.QueryServices;

public class DriverQueryService(IDriverRepository driverRepository) : IDriverQueryService
{
    public async Task<IEnumerable<Driver>> Handle(GetAllDriversQuery query) =>
        (await driverRepository.ListAsync()).OrderBy(d => d.Name).ThenBy(d => d.Id);

    public Task<Driver?> Handle(GetDriverByIdQuery query) => driverRepository.FindByIdAsync(query.DriverId);
}

public class VehicleQueryService(IVehicleRepository vehicleRepository) : IVehicleQueryService
{
    public async Task<IEnumerable<Vehicle>> Handle(GetAllVehiclesQuery query) =>
        (await vehicleRepository.ListAsync()).OrderBy(v => v.UnitCode).ThenBy(v => v.Id);

    public Task<Vehicle?> Handle(GetVehicleByIdQuery query) => vehicleRepository.FindByIdAsync(query.VehicleId);
}
