using RebooTech.Fruitlogix.Platform.Fleet.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;

namespace RebooTech.Fruitlogix.Platform.Fleet.Application.Internal.CommandServices;

public class DriverCommandService(IDriverRepository driverRepository, IUnitOfWork unitOfWork) : IDriverCommandService
{
    public async Task<Driver> Handle(CreateDriverCommand command)
    {
        var driver = new Driver(command);
        await driverRepository.AddAsync(driver);
        await unitOfWork.CompleteAsync();
        return driver;
    }

    public async Task<Driver?> Handle(UpdateDriverCommand command)
    {
        var driver = await driverRepository.FindByIdAsync(command.DriverId);
        if (driver is null) return null;
        driver.Update(command);
        driverRepository.Update(driver);
        await unitOfWork.CompleteAsync();
        return driver;
    }

    public async Task<bool> Handle(DeleteDriverCommand command)
    {
        var driver = await driverRepository.FindByIdAsync(command.DriverId);
        if (driver is null) return false;
        driverRepository.Remove(driver);
        await unitOfWork.CompleteAsync();
        return true;
    }
}

public class VehicleCommandService(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork) : IVehicleCommandService
{
    public async Task<Vehicle> Handle(CreateVehicleCommand command)
    {
        var vehicle = new Vehicle(command);
        await EnsurePlateIsFreeAsync(vehicle.Plate, null);
        await vehicleRepository.AddAsync(vehicle);
        await unitOfWork.CompleteAsync();
        return vehicle;
    }

    public async Task<Vehicle?> Handle(UpdateVehicleCommand command)
    {
        var vehicle = await vehicleRepository.FindByIdAsync(command.VehicleId);
        if (vehicle is null) return null;
        vehicle.Update(command);
        await EnsurePlateIsFreeAsync(vehicle.Plate, vehicle.Id);
        vehicleRepository.Update(vehicle);
        await unitOfWork.CompleteAsync();
        return vehicle;
    }

    public async Task<bool> Handle(DeleteVehicleCommand command)
    {
        var vehicle = await vehicleRepository.FindByIdAsync(command.VehicleId);
        if (vehicle is null) return false;
        vehicleRepository.Remove(vehicle);
        await unitOfWork.CompleteAsync();
        return true;
    }

    private async Task EnsurePlateIsFreeAsync(string plate, int? excludeVehicleId)
    {
        if (await vehicleRepository.ExistsByPlateAsync(plate, excludeVehicleId))
            throw new InvalidOperationException($"A vehicle with plate {plate} already exists.");
    }
}
