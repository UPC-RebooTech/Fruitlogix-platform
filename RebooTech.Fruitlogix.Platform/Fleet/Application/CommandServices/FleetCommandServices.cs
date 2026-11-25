using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;

namespace RebooTech.Fruitlogix.Platform.Fleet.Application.CommandServices;

public interface IDriverCommandService
{
    Task<Driver> Handle(CreateDriverCommand command);
    Task<Driver?> Handle(UpdateDriverCommand command);
    /// <summary>false si no existe.</summary>
    Task<bool> Handle(DeleteDriverCommand command);
}

public interface IVehicleCommandService
{
    /// <exception cref="InvalidOperationException">La placa ya existe (409).</exception>
    Task<Vehicle> Handle(CreateVehicleCommand command);
    /// <exception cref="InvalidOperationException">La placa ya existe en otra unidad (409).</exception>
    Task<Vehicle?> Handle(UpdateVehicleCommand command);
    /// <summary>false si no existe.</summary>
    Task<bool> Handle(DeleteVehicleCommand command);
}
