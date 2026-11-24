using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;

public class Delivery : IAuditableEntity
{
    public int    Id                      { get; private set; }
    public int    OrderId                 { get; private set; }
    public DriverInfo  Driver             { get; private set; } = null!;
    public VehicleInfo Vehicle            { get; private set; } = null!;
    public RouteInfo   Route              { get; private set; } = null!;
    public DateTime    EstimatedTimeOfArrival { get; private set; }
    public DeliveryStatus CurrentStatus   { get; private set; }
    /// <summary>Usuario IAM distribuidor asignado (null en datos antiguos).</summary>
    public int?   DistributorUserId       { get; private set; }
    public DateTimeOffset? DispatchedAt { get; private set; }
    public DateTimeOffset? ArrivedAt    { get; private set; }
    public DateTimeOffset? DeliveredAt  { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Constructor vacío para EF Core
    protected Delivery() { }

    public Delivery(CreateDeliveryCommand command)
    {
        OrderId                = command.OrderId;
        Driver                 = new DriverInfo(command.DriverName, command.DriverPhone);
        Vehicle                = new VehicleInfo(command.VehiclePlate, command.VehicleType);
        Route                  = new RouteInfo(command.RouteOrigin, command.RouteDestination, command.RouteDistanceKm);
        EstimatedTimeOfArrival = command.EstimatedTimeOfArrival;
        CurrentStatus          = DeliveryStatus.PendingDispatch;
        DistributorUserId      = command.DistributorUserId;
    }

    public void UpdateStatus(DeliveryStatus newStatus)
    {
        CurrentStatus = newStatus;
    }
    
    public string? DelayReason { get; private set; }

    public void StartDispatch()
    {
        if (CurrentStatus != DeliveryStatus.PendingDispatch)
            throw new InvalidOperationException("Delivery must be in PendingDispatch to start.");
        CurrentStatus = DeliveryStatus.InTransit;
        DispatchedAt  = DateTimeOffset.UtcNow;
    }

    public void ReportDelay(string reason)
    {
        if (CurrentStatus is not (DeliveryStatus.PendingDispatch or DeliveryStatus.InTransit or DeliveryStatus.Delayed))
            throw new InvalidOperationException("A delay can only be reported before the delivery arrives.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Delay reason is required.");
        CurrentStatus = DeliveryStatus.Delayed;
        DelayReason   = reason.Trim();
    }

    /// <summary>InTransit o Delayed → Arrived.</summary>
    public void Arrive()
    {
        if (CurrentStatus is not (DeliveryStatus.InTransit or DeliveryStatus.Delayed))
            throw new InvalidOperationException("Delivery must be in transit or delayed to mark arrival.");
        CurrentStatus = DeliveryStatus.Arrived;
        ArrivedAt     = DateTimeOffset.UtcNow;
    }

    /// <summary>La recepción del cliente cierra la entrega, venga del estado que venga.</summary>
    public void MarkDelivered()
    {
        CurrentStatus = DeliveryStatus.Delivered;
        DeliveredAt   = DateTimeOffset.UtcNow;
    }

    public void EnsureCanRecordCheckpoint()
    {
        if (CurrentStatus is not (DeliveryStatus.InTransit or DeliveryStatus.Delayed))
            throw new InvalidOperationException("Checkpoints can only be recorded while the delivery is in transit or delayed.");
    }

    /// <summary>Cambia conductor y/o vehículo; los valores null conservan el actual.</summary>
    public void UpdateAssignment(string? driverName, string? driverPhone, string? vehiclePlate, string? vehicleType)
    {
        if (CurrentStatus is DeliveryStatus.Arrived or DeliveryStatus.Delivered or DeliveryStatus.Failed)
            throw new InvalidOperationException("The assignment cannot change after the delivery arrived.");
        Driver  = new DriverInfo(driverName ?? Driver.Name, driverPhone ?? Driver.Phone);
        Vehicle = new VehicleInfo(vehiclePlate ?? Vehicle.Plate, vehicleType ?? Vehicle.Type);
    }
}
