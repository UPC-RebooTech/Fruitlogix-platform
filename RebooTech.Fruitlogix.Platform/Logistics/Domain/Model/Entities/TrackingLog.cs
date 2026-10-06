using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Model;

namespace RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;

public class TrackingLog : IAuditableEntity
{
    public int    Id           { get; private set; }
    public int    DeliveryId   { get; private set; }
    public DateTime  Timestamp { get; private set; }
    public GpsCoordinates Location    { get; private set; } = null!;
    public double Temperature { get; private set; }
    public double Humidity    { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected TrackingLog() { }

    public TrackingLog(CreateTrackingLogCommand command)
    {
        DeliveryId  = command.DeliveryId;
        Timestamp   = command.Timestamp;
        Location    = new GpsCoordinates(command.Latitude, command.Longitude);
        Temperature = command.Temperature;
        Humidity    = command.Humidity;
    }
}