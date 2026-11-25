using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Queries;

namespace RebooTech.Fruitlogix.Platform.Fleet.Application.QueryServices;

public interface IDriverQueryService
{
    Task<IEnumerable<Driver>> Handle(GetAllDriversQuery query);
    Task<Driver?> Handle(GetDriverByIdQuery query);
}

public interface IVehicleQueryService
{
    Task<IEnumerable<Vehicle>> Handle(GetAllVehiclesQuery query);
    Task<Vehicle?> Handle(GetVehicleByIdQuery query);
}
