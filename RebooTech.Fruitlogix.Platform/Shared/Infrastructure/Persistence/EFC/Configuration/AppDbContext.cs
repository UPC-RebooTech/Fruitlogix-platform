using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Aggregates;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Model.Entities;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Converters;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <summary>
    ///     Todas las propiedades DateOnly se guardan como DATE pasando por DateTime: el proveedor MySQL
    ///     devuelve DateTime para DATE y la lectura directa como DateOnly falla (InvalidCastException).
    ///     Los value objects DeliveryDate y DueDate tienen su propio HasConversion a DateTime.
    /// </summary>
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);
        builder.Properties<DateOnly>().HaveConversion<DateOnlyToDateTimeConverter>().HaveColumnType("date");
        builder.Properties<DateOnly?>().HaveConversion<NullableDateOnlyToDateTimeConverter>().HaveColumnType("date");
    }

    // 🌟 AQUÍ ESTÁ EL DBSET PARA LOS USUARIOS DEL IAM
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Producer.OperationsStartDate (DateOnly?) usa la convención de ConfigureConventions.

        // Order Management
        builder.Entity<Order>().HasKey(o => o.Id);
        builder.Entity<Order>().Property(o => o.Id)
            .IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Order>().Property(o => o.CommercialClientId)
            .IsRequired();
        builder.Entity<Order>().Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();
            
        builder.Entity<Order>().Property(o => o.DeliveryAddress)
            .HasMaxLength(255)
            .IsRequired();

        builder.Entity<Order>().Property(o => o.TotalAmount)
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        builder.Entity<Order>().Property(o => o.Notes)
            .IsRequired(false);
        builder.Entity<Order>().Property(o => o.CancellationReason)
            .IsRequired(false);
        builder.Entity<Order>().Property(o => o.ReceptionRating).IsRequired(false);
        builder.Entity<Order>().Property(o => o.ReceptionComment)
            .HasMaxLength(Order.MaxReceptionCommentLength).IsRequired(false);
        builder.Entity<Order>().Property(o => o.SealAndTemperatureOk).IsRequired(false);
        builder.Entity<Order>().Property(o => o.DeliveredAt).IsRequired(false);
        builder.Entity<Order>().Property(o => o.BatchId).IsRequired(false);
        builder.Entity<Order>().HasIndex(o => o.BatchId);
        builder.Entity<Order>()
            .Property(o => o.ProducerId)
            .HasConversion(
                v => v == null ? (int?)null : v.Value,
                v => v == null ? null : new ProducerId(v.Value))
            .IsRequired(false);
        builder.Entity<Order>()
            .Property(o => o.DeliveryDueDate)
            .HasConversion(
                v => v.Value.ToDateTime(TimeOnly.MinValue),
                v => DeliveryDate.FromDatabase(DateOnly.FromDateTime(v)))
            .HasColumnType("date")
            .IsRequired();
        builder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Order Items
        builder.Entity<OrderItem>().HasKey(i => i.Id);
        builder.Entity<OrderItem>().Property(i => i.Id)
            .IsRequired().ValueGeneratedOnAdd();
            
        builder.Entity<OrderItem>().Property(i => i.ProductId)
            .IsRequired();
            
        builder.Entity<OrderItem>().Property(i => i.ProductName)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Entity<OrderItem>().Property(i => i.QuantityKg)
            .IsRequired();
        builder.Entity<OrderItem>().Property(i => i.UnitPrice)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
            
        builder.Entity<OrderItem>().Property(i => i.Subtotal)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
            
        // Profiles
        builder.Entity<Producer>().HasKey(p => p.Id);
        builder.Entity<Producer>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<Producer>().Property(p => p.ProducerType)
            .HasConversion<string>().IsRequired();

        builder.Entity<Producer>().Property(p => p.FullName).IsRequired();
        builder.Entity<Producer>().Property(p => p.LegalName).IsRequired();
        builder.Entity<Producer>().Property(p => p.Rating).IsRequired();
        builder.Entity<Producer>().Property(p => p.Certifications);
        builder.Entity<Producer>().Property(p => p.Photo);
        builder.Entity<Producer>().Property(p => p.UserId).IsRequired(false);
        builder.Entity<Producer>().HasIndex(p => p.UserId);

        builder.Entity<Producer>().OwnsOne(p => p.TaxId, vo =>
        {
            vo.Property(t => t.Value).HasColumnName("tax_id").IsRequired();
            vo.WithOwner().HasForeignKey("Id");
        });

        builder.Entity<Producer>().OwnsOne(p => p.ContactInfo, vo =>
        {
            vo.Property(c => c.Email).HasColumnName("email").IsRequired();
            vo.Property(c => c.Phone).HasColumnName("phone").IsRequired();
            vo.WithOwner().HasForeignKey("Id");
        });

        builder.Entity<Producer>().OwnsOne(p => p.Location, vo =>
        {
            vo.Property(l => l.Country).HasColumnName("country");
            vo.Property(l => l.Region).HasColumnName("region");
            vo.Property(l => l.City).HasColumnName("city");
            vo.Property(l => l.Address).HasColumnName("address");
            vo.WithOwner().HasForeignKey("Id");
        });
        
        builder.Entity<Producer>().OwnsOne(p => p.ProductionInfo, vo =>
        {
            vo.Property(pi => pi.Crop).HasColumnName("crop").IsRequired();
            vo.Property(pi => pi.CultivatedHectares).HasColumnName("cultivated_hectares");
            vo.Property(pi => pi.MonthlyProduction).HasColumnName("monthly_production");
            vo.WithOwner().HasForeignKey("Id");
        });

        // Harvest plans (cosechas programadas)
        builder.Entity<HarvestPlan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(p => p.ProducerId).IsRequired();
            entity.HasIndex(p => p.ProducerId);
            entity.Property(p => p.FruitType).IsRequired().HasMaxLength(HarvestPlan.MaxFruitTypeLength);
            entity.Property(p => p.ScheduledFor).IsRequired();
            entity.Property(p => p.EstimatedYieldKg).IsRequired();
            entity.Property(p => p.Zone).IsRequired().HasMaxLength(HarvestPlan.MaxZoneLength);
            entity.Property(p => p.CrewName).IsRequired().HasMaxLength(HarvestPlan.MaxCrewNameLength);
            entity.Property(p => p.Pickers).IsRequired();
            entity.Property(p => p.Notes).HasMaxLength(HarvestPlan.MaxNotesLength).IsRequired(false);
        });

        // Commercial client profile
        builder.Entity<CommercialClient>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(c => c.UserId).IsRequired();
            entity.HasIndex(c => c.UserId).IsUnique();
            entity.Property(c => c.BusinessName).HasMaxLength(CommercialClient.MaxBusinessNameLength).IsRequired(false);
            entity.Property(c => c.Ruc).HasMaxLength(CommercialClient.MaxRucLength).IsRequired(false);
            entity.Property(c => c.ContactName).HasMaxLength(CommercialClient.MaxContactNameLength).IsRequired(false);
            entity.Property(c => c.Email).HasMaxLength(CommercialClient.MaxEmailLength).IsRequired(false);
            entity.Property(c => c.Phone).HasMaxLength(CommercialClient.MaxPhoneLength).IsRequired(false);
            entity.Property(c => c.Address).HasMaxLength(CommercialClient.MaxAddressLength).IsRequired(false);
            entity.Property(c => c.WarehouseLatitude).IsRequired(false);
            entity.Property(c => c.WarehouseLongitude).IsRequired(false);
            entity.Property(c => c.WarehouseRadiusKm).IsRequired(false);
            entity.Ignore(c => c.HasWarehouse);
        });

        // Quality Control
        builder.Entity<HarvestBatch>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();

            entity.Property(b => b.FruitType).IsRequired().HasMaxLength(100);
            entity.Property(b => b.QuantityKg).IsRequired();
            entity.Property(b => b.HarvestDate).IsRequired();
            entity.Property(b => b.Status)
                .HasConversion<string>() 
                .IsRequired();
        }); 
        
        builder.Entity<QualityInspection>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Notes).HasMaxLength(500);
            entity.Property(q => q.InspectorUserId).IsRequired(false);
            entity.Property(q => q.InspectorName).HasMaxLength(QualityInspection.MaxInspectorNameLength).IsRequired(false);

            entity.HasOne(q => q.VisualInspection)
                .WithOne()
                .HasForeignKey<VisualInspection>(v => v.QualityInspectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quality_inspections_visual"); 

            entity.HasOne(q => q.TechnicalParameters)
                .WithOne()
                .HasForeignKey<TechnicalParameters>(t => t.QualityInspectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quality_inspections_technical"); 
    
            entity.HasOne(q => q.PreparationChecklist)
                .WithOne()
                .HasForeignKey<PreparationChecklist>(c => c.QualityInspectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quality_inspections_preparation"); 
        });

        builder.Entity<VisualInspection>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.WastePercentage).IsRequired();
            entity.Property(v => v.AppearanceRating).IsRequired();
        });

        builder.Entity<TechnicalParameters>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.TemperatureCelsius).IsRequired();
            entity.Property(t => t.HumidityPercent).IsRequired();
            entity.Property(t => t.Ph).IsRequired();
            entity.Property(t => t.BrixDegrees).IsRequired();
        });

        builder.Entity<PreparationChecklist>(entity =>
        {
            entity.HasKey(c => c.Id);
        });
        
        builder.Entity<Incident>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();

            entity.Property(i => i.BatchId).IsRequired();
            entity.Property(i => i.Description).IsRequired().HasMaxLength(1000);
            entity.Property(i => i.EvidenceUrls).IsRequired();

            entity.Property(i => i.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(i => i.ResolvedAt);
            entity.Property(i => i.ReportedByUserId).IsRequired(false);
            entity.Property(i => i.OrderId).IsRequired(false);
            entity.HasIndex(i => i.ReportedByUserId);
            entity.HasIndex(i => i.OrderId);
        });

        // Delivery
        builder.Entity<Delivery>().HasKey(d => d.Id);
        builder.Entity<Delivery>().Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Delivery>().Property(d => d.OrderId).IsRequired();
        builder.Entity<Delivery>().Property(d => d.EstimatedTimeOfArrival).IsRequired();
        builder.Entity<Delivery>().Property(d => d.CurrentStatus)
            .HasConversion<string>().IsRequired();

        builder.Entity<Delivery>().OwnsOne(d => d.Driver, nav =>
        {
            nav.Property(x => x.Name).HasColumnName("driver_name").IsRequired();
            nav.Property(x => x.Phone).HasColumnName("driver_phone");
            nav.WithOwner().HasForeignKey("Id"); 
        });

        builder.Entity<Delivery>().OwnsOne(d => d.Vehicle, nav =>
        {
            nav.Property(x => x.Plate).HasColumnName("vehicle_plate").IsRequired();
            nav.Property(x => x.Type).HasColumnName("vehicle_type");
            nav.WithOwner().HasForeignKey("Id"); 
        });

        builder.Entity<Delivery>().OwnsOne(d => d.Route, nav =>
        {
            nav.Property(x => x.Origin).HasColumnName("route_origin").IsRequired();
            nav.Property(x => x.Destination).HasColumnName("route_destination").IsRequired();
            nav.Property(x => x.DistanceKm).HasColumnName("route_distance_km").IsRequired();
            nav.WithOwner().HasForeignKey("Id"); 
        }); 
        builder.Entity<Delivery>().Property(d => d.DelayReason);
        builder.Entity<Delivery>().Property(d => d.DistributorUserId).IsRequired(false);
        builder.Entity<Delivery>().Property(d => d.DispatchedAt).IsRequired(false);
        builder.Entity<Delivery>().Property(d => d.ArrivedAt).IsRequired(false);
        builder.Entity<Delivery>().Property(d => d.DeliveredAt).IsRequired(false);
        builder.Entity<Delivery>().Property(d => d.DriverId).IsRequired(false);
        builder.Entity<Delivery>().Property(d => d.VehicleId).IsRequired(false);
        builder.Entity<Delivery>().HasIndex(d => d.DriverId);
        builder.Entity<Delivery>().HasIndex(d => d.VehicleId);

        builder.Entity<DeliveryEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.DeliveryId).IsRequired();
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(DeliveryEvent.MaxNameLength).IsRequired(false);
            entity.Property(e => e.Note).HasMaxLength(DeliveryEvent.MaxNoteLength).IsRequired(false);
            entity.Property(e => e.Latitude).IsRequired(false);
            entity.Property(e => e.Longitude).IsRequired(false);
            entity.Property(e => e.OccurredAt).IsRequired();
            entity.Property(e => e.CreatedByUserId).IsRequired(false);
            entity.HasIndex(e => e.DeliveryId);
        });
        builder.Entity<Delivery>().HasIndex(d => d.DistributorUserId);

        builder.Entity<TrackingLog>().HasKey(t => t.Id);
        builder.Entity<TrackingLog>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TrackingLog>().Property(t => t.DeliveryId).IsRequired();
        builder.Entity<TrackingLog>().Property(t => t.Timestamp).IsRequired();
        builder.Entity<TrackingLog>().Property(t => t.Temperature).IsRequired();
        builder.Entity<TrackingLog>().Property(t => t.Humidity).IsRequired();

        builder.Entity<TrackingLog>().OwnsOne(t => t.Location, nav =>
        {
            nav.Property(x => x.Latitude).HasColumnName("latitude").IsRequired();
            nav.Property(x => x.Longitude).HasColumnName("longitude").IsRequired();
            nav.WithOwner().HasForeignKey("Id");
        });
        
        builder.Entity<Alert>().HasKey(a => a.Id);
        builder.Entity<Alert>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Alert>().Property(a => a.DeliveryId).IsRequired();
        builder.Entity<Alert>().Property(a => a.Type).HasConversion<string>().IsRequired();
        builder.Entity<Alert>().Property(a => a.Severity).HasConversion<string>().IsRequired();
        builder.Entity<Alert>().Property(a => a.Message).IsRequired();
        builder.Entity<Alert>().Property(a => a.IsResolved).IsRequired();
        
        // Payments
        builder.Entity<Invoice>().HasKey(i => i.Id);
        builder.Entity<Invoice>().Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Invoice>().Property(i => i.ClientId).IsRequired();
        builder.Entity<Invoice>().Property(i => i.OrderId).IsRequired(false);

        builder.Entity<Invoice>().OwnsOne(i => i.TotalAmount, money =>
        {
            money.WithOwner().HasForeignKey("Id");
            money.Property(m => m.Amount).HasColumnName("total_amount").IsRequired();
            money.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        });

        builder.Entity<Invoice>()
            .Property(i => i.DueDate)
            .HasConversion(
                v => v.Value.ToDateTime(TimeOnly.MinValue),
                v => DueDate.FromDatabase(DateOnly.FromDateTime(v)))
            .HasColumnType("date")
            .IsRequired();

        builder.Entity<Invoice>().Property(i => i.Status).HasConversion<string>().IsRequired();
        builder.Entity<Invoice>().Property(i => i.InvoiceType).HasConversion<string>().IsRequired();
        builder.Entity<Invoice>().Property(i => i.IssuedAt).IsRequired(false);
        builder.Entity<Invoice>().Property(i => i.PaidAt).IsRequired(false);
        builder.Entity<Invoice>().Property(i => i.IssuedByUserId).IsRequired(false);
        
        builder.Entity<PaymentTransaction>().HasKey(t => t.Id);
        builder.Entity<PaymentTransaction>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PaymentTransaction>().Property(t => t.InvoiceId).IsRequired();
        builder.Entity<PaymentTransaction>().Property(t => t.GatewayRef).IsRequired().HasMaxLength(100);
        builder.Entity<PaymentTransaction>().Property(t => t.ProcessedAt).IsRequired();

        builder.Entity<PaymentTransaction>().OwnsOne(t => t.Amount, money =>
        {
            money.WithOwner().HasForeignKey("Id");
            money.Property(m => m.Amount).HasColumnName("amount").IsRequired();
            money.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        });

        builder.Entity<PaymentTransaction>().OwnsOne(t => t.CardDetails, card =>
        {
            card.WithOwner().HasForeignKey("Id");
            card.Property(c => c.CardEnding).HasColumnName("card_ending").HasMaxLength(4).IsRequired(false);
            card.Property(c => c.CardBrand).HasColumnName("card_brand").HasMaxLength(20).IsRequired(false);
        });

        builder.Entity<PaymentTransaction>().Property(t => t.Method).HasConversion<string>().IsRequired();
        builder.Entity<PaymentTransaction>().Property(t => t.Gateway).HasConversion<string>().IsRequired();
        builder.Entity<PaymentTransaction>().Property(t => t.Status).HasConversion<string>().IsRequired();

        builder.Entity<Membership>().HasKey(m => m.Id);
        builder.Entity<Membership>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Membership>().Property(m => m.UserId).IsRequired();
        builder.Entity<Membership>().HasIndex(m => m.UserId).IsUnique();
        builder.Entity<Membership>().Property(m => m.Plan).HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Entity<Membership>().Property(m => m.GatewayRef).HasMaxLength(100).IsRequired(false);
        builder.Entity<Membership>().Property(m => m.ActivatedAt).IsRequired(false);
        
        // IoTInfrastructure
        builder.Entity<IoTDevice>().HasKey(d => d.Id);
        builder.Entity<IoTDevice>().Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<IoTDevice>().OwnsOne(d => d.DeviceCode, dc =>
        {
            dc.WithOwner().HasForeignKey("Id");
            dc.Property(v => v.Value).HasColumnName("device_code").IsRequired();
        });

        builder.Entity<IoTDevice>()
            .Property(d => d.DeviceType)
            .HasConversion<string>()
            .IsRequired();

        builder.Entity<IoTDevice>()
            .Property(d => d.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Entity<IoTDevice>()
            .Property(d => d.Location)
            .IsRequired();

        builder.Entity<IoTDevice>()
            .Property(d => d.LastReadingAt)
            .IsRequired(false);
        
        builder.Entity<SensorReading>().HasKey(r => r.Id);
        builder.Entity<SensorReading>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SensorReading>().Property(r => r.DeviceId).IsRequired();
        builder.Entity<SensorReading>().Property(r => r.Timestamp).IsRequired();

        builder.Entity<SensorReading>().OwnsOne(r => r.Reading, rv =>
        {
            rv.WithOwner().HasForeignKey("Id");
            rv.Property(v => v.Value).HasColumnName("value").IsRequired();
            rv.Property(v => v.Unit).HasColumnName("unit").IsRequired();
        });
        
        builder.Entity<AlertRule>().HasKey(r => r.Id);
        builder.Entity<AlertRule>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<AlertRule>().OwnsOne(r => r.Threshold, nav =>
        {
            nav.WithOwner().HasForeignKey("Id");
            nav.Property(v => v.Min).HasColumnName("min_threshold").IsRequired();
            nav.Property(v => v.Max).HasColumnName("max_threshold").IsRequired();
        });

        builder.Entity<AlertRule>()
            .Property(r => r.DeviceType)
            .HasConversion<string>()
            .IsRequired();

        builder.Entity<AlertRule>()
            .Property(r => r.Severity)
            .HasConversion<string>()
            .IsRequired();

        builder.Entity<AlertRule>()
            .Property(r => r.AlertMessage)
            .IsRequired();

        builder.Entity<AlertRule>()
            .Property(r => r.IsActive)
            .IsRequired();
        
        // Messaging
        builder.Entity<Conversation>().HasKey(c => c.Id);
        builder.Entity<Conversation>().Property(c => c.Id)
            .IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Conversation>().Property(c => c.OrderId)
            .IsRequired();
        builder.Entity<Conversation>().Property(c => c.ParticipantAId)
            .IsRequired();
        builder.Entity<Conversation>().Property(c => c.ParticipantBId)
            .IsRequired();

        builder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey("ConversationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>().HasKey(m => m.Id);
        builder.Entity<Message>().Property(m => m.Id)
            .IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Message>().Property(m => m.SenderId)
            .IsRequired();
        builder.Entity<Message>().Property(m => m.SentAt)
            .IsRequired();

        builder.Entity<Message>().OwnsOne(m => m.Content, content =>
        {
            content.WithOwner().HasForeignKey("Id");
            content.Property(c => c.Value)
                .HasColumnName("content")
                .IsRequired();
        });

        // Product Catalog
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(p => p.ProducerId).IsRequired();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(Product.MaxNameLength);
            entity.Property(p => p.FruitType).IsRequired().HasMaxLength(Product.MaxFruitTypeLength);
            entity.Property(p => p.Variety).HasMaxLength(Product.MaxVarietyLength).IsRequired(false);
            entity.Property(p => p.Description).HasMaxLength(Product.MaxDescriptionLength).IsRequired(false);
            entity.Property(p => p.UnitPrice).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(p => p.AvailableKg).IsRequired();
            entity.Property(p => p.MinOrderKg).IsRequired();
            entity.Property(p => p.ImageUrl).HasMaxLength(Product.MaxImageUrlLength).IsRequired(false);
            entity.Property(p => p.IsActive).IsRequired();
            entity.Ignore(p => p.IsAvailable);
            entity.HasIndex(p => p.ProducerId);
            entity.HasIndex(p => p.FruitType);
        });

        // Fleet
        builder.Entity<Driver>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(d => d.Name).IsRequired().HasMaxLength(Driver.MaxNameLength);
            entity.Property(d => d.Phone).HasMaxLength(Driver.MaxPhoneLength).IsRequired(false);
            entity.Property(d => d.LicenseClass).IsRequired().HasMaxLength(Driver.MaxLicenseClassLength);
            entity.Property(d => d.LicenseExpiry).IsRequired();
            entity.Property(d => d.ColdChainCertified).IsRequired();
            entity.Property(d => d.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(d => d.RestUntil).IsRequired(false);
            entity.Property(d => d.CreatedByUserId).IsRequired(false);
        });

        builder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(v => v.UnitCode).IsRequired().HasMaxLength(Vehicle.MaxUnitCodeLength);
            entity.Property(v => v.Plate).IsRequired().HasMaxLength(Vehicle.PlateLength);
            entity.HasIndex(v => v.Plate).IsUnique();
            entity.Property(v => v.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(v => v.CapacityKg).IsRequired();
            entity.Property(v => v.MinCelsius).IsRequired();
            entity.Property(v => v.MaxCelsius).IsRequired();
            entity.Property(v => v.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(v => v.CreatedByUserId).IsRequired(false);
        });

        // -- IAM CONFIGURATION --
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<User>().Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.UseSnakeCaseNamingConvention();
    }
}