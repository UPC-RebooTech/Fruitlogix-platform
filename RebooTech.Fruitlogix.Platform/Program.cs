using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.IoTInfrastructure.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Logistics.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Logistics.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Logistics.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Messaging.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Messaging.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.Messaging.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.Messaging.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Messaging.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Messaging.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.OrderManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.OrderManagement.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.PaymentManagement.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Profiles.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Profiles.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.Profiles.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Profiles.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Profiles.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Profiles.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.ProductCatalog.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.QualityControl.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.QualityControl.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Authorization;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Geofencing;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Traceability;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Naming;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.FleetUsage;
using RebooTech.Fruitlogix.Platform.Fleet.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Fleet.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Fleet.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.Fleet.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.Fleet.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Fleet.Infrastructure.Persistence.EFC.Repositories;
using RebooTech.Fruitlogix.Platform.Shared.Application.Internal.OutboundServices;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.FileStorage.Cloudinary;
using RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Interfaces.ASP.Configuration;
using RebooTech.Fruitlogix.Platform.Iam.Application.Acl;
using RebooTech.Fruitlogix.Platform.Iam.Application.CommandServices;
using RebooTech.Fruitlogix.Platform.Iam.Application.Internal.CommandServices;
using RebooTech.Fruitlogix.Platform.Iam.Application.Internal.OutboundServices;
using RebooTech.Fruitlogix.Platform.Iam.Application.Internal.QueryServices;
using RebooTech.Fruitlogix.Platform.Iam.Application.QueryServices;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Repositories;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;
using RebooTech.Fruitlogix.Platform.Iam.Infrastructure.Tokens.Jwt.Services;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Acl;
using Microsoft.OpenApi;
using RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);   

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));

// ── Shared ────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccessScopeService, AccessScopeService>();
builder.Services.AddScoped<IOrderDependencyService, OrderDependencyService>();
builder.Services.AddScoped<IOrderTraceabilityService, OrderTraceabilityService>();
builder.Services.AddScoped<IClientNameService, ClientNameService>();
builder.Services.AddScoped<IFleetUsageService, FleetUsageService>();

// ── Fleet BC (choferes y vehículos) ──
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDriverCommandService, DriverCommandService>();
builder.Services.AddScoped<IDriverQueryService, DriverQueryService>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleCommandService, VehicleCommandService>();
builder.Services.AddScoped<IVehicleQueryService, VehicleQueryService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<IFileStorageService, CloudinaryFileStorageService>();
builder.Services.AddSingleton<RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory>();
// ── Order Management ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderCommandService, OrderCommandService>();
builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();

// ── Profiles BC ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<IProducerRepository, ProducerRepository>();
builder.Services.AddScoped<IProducerCommandService, ProducerCommandService>();
builder.Services.AddScoped<IProducerQueryService, ProducerQueryService>();
builder.Services.AddScoped<ICommercialClientRepository, CommercialClientRepository>();
builder.Services.AddScoped<ICommercialClientCommandService, CommercialClientCommandService>();
builder.Services.AddScoped<ICommercialClientQueryService, CommercialClientQueryService>();

// ── Product Catalog BC ────────────────────────────────────────────────────────
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();

// ── Quality Control BC ────────────────────────────────────────────────────────
builder.Services.AddScoped<IHarvestBatchRepository, HarvestBatchRepository>();
builder.Services.AddScoped<IHarvestBatchCommandService, HarvestBatchCommandService>();
builder.Services.AddScoped<IHarvestBatchQueryService, HarvestBatchQueryService>();
builder.Services.AddScoped<IHarvestPlanRepository, HarvestPlanRepository>();
builder.Services.AddScoped<IHarvestPlanCommandService, HarvestPlanCommandService>();
builder.Services.AddScoped<IHarvestPlanQueryService, HarvestPlanQueryService>();
builder.Services.AddScoped<IQualityInspectionRepository, QualityInspectionRepository>();
builder.Services.AddScoped<IQualityInspectionCommandService, QualityInspectionCommandService>();
builder.Services.AddScoped<IQualityInspectionQueryService, QualityInspectionQueryService>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentCommandService, IncidentCommandService>();
builder.Services.AddScoped<IIncidentQueryService, IncidentQueryService>();

// ── Logistics ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IDeliveryCommandService, DeliveryCommandService>();
builder.Services.AddScoped<IDeliveryQueryService, DeliveryQueryService>();
builder.Services.AddScoped<IDeliveryEventRepository, DeliveryEventRepository>();
builder.Services.AddScoped<IWarehouseGeofenceService, WarehouseGeofenceService>();
builder.Services.AddScoped<ITrackingLogRepository, TrackingLogRepository>();
builder.Services.AddScoped<ITrackingLogCommandService, TrackingLogCommandService>();
builder.Services.AddScoped<ITrackingLogQueryService, TrackingLogQueryService>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();

// ── Payment ───────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceCommandService, InvoiceCommandService>();
builder.Services.AddScoped<IInvoiceQueryService, InvoiceQueryService>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
builder.Services.AddScoped<IPaymentTransactionCommandService, PaymentTransactionCommandService>();
builder.Services.AddScoped<IPaymentTransactionQueryService, PaymentTransactionQueryService>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IMembershipCommandService, MembershipCommandService>();
builder.Services.AddScoped<IMembershipQueryService, MembershipQueryService>();

// ── IoTInfrastructure ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IIoTDeviceRepository, IoTDeviceRepository>();
builder.Services.AddScoped<IIoTDeviceQueryService, IoTDeviceQueryService>();
builder.Services.AddScoped<IIoTDeviceCommandService, IoTDeviceCommandService>();
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<ISensorReadingCommandService, SensorReadingCommandService>();
builder.Services.AddScoped<ISensorReadingQueryService, SensorReadingQueryService>();
builder.Services.AddScoped<IAlertRuleRepository, AlertRuleRepository>();
builder.Services.AddScoped<IAlertRuleCommandService, AlertRuleCommandService>();
builder.Services.AddScoped<IAlertRuleQueryService, AlertRuleQueryService>();

// ── Messaging ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationCommandService, ConversationCommandService>();
builder.Services.AddScoped<IMessageCommandService, MessageCommandService>();
builder.Services.AddScoped<IMessageQueryService, MessageQueryService>();
builder.Services.AddScoped<IConversationQueryService, ConversationQueryService>();

//  ── IAM (Identity and Access Management) ──────────────────────────────────
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers(options =>
        options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ── Swagger con Soporte para Tokens JWT ───────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SchemaFilter<EnumValuesSchemaFilter>();
    // Comentarios /// de controllers y resources (GenerateDocumentationFile en el .csproj)
    var xmlDocs = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    if (File.Exists(xmlDocs)) options.IncludeXmlComments(xmlDocs, includeControllerXmlComments: true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() }
    });
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                "http://localhost:5173",   
                "http://localhost:4173",   
                "https://fruitlogix.vercel.app", 
                "https://fruitlogixweb.web.app"  
            ) 
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// ── Localización (Traducciones y Errores del Profesor) ────────────────────────
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddSingleton<
    Microsoft.Extensions.Localization.IStringLocalizer<RebooTech.Fruitlogix.Platform.Resources.Errors.ErrorMessages>, 
    Microsoft.Extensions.Localization.StringLocalizer<RebooTech.Fruitlogix.Platform.Resources.Errors.ErrorMessages>>();

builder.Services.AddSingleton<
    Microsoft.Extensions.Localization.IStringLocalizer<RebooTech.Fruitlogix.Platform.Resources.Shared.CommonMessages>, 
    Microsoft.Extensions.Localization.StringLocalizer<RebooTech.Fruitlogix.Platform.Resources.Shared.CommonMessages>>();

Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// ── Errores no controlados → 500 con ProblemDetails (con traceId) ─────────────
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Pipeline.GlobalExceptionHandler>();

var app = builder.Build();

// Primero en el pipeline: captura también lo que lancen Swagger, CORS y el middleware de autorización.
app.UseExceptionHandler();

// ── Migrations automáticas ────────────────────────────────────────────────────
// La BD (filess.io) limita el usuario a 5 conexiones simultáneas. Durante un deploy la instancia
// anterior (o una API local) puede tenerlas ocupadas, así que se reintenta en vez de caer al primer fallo.
const int migrationMaxAttempts = 8;
var migrationRetryDelay = TimeSpan.FromSeconds(15);
for (var attempt = 1; ; attempt++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        break;
    }
    catch (Exception ex) when (attempt < migrationMaxAttempts && IsTransientConnectionError(ex))
    {
        app.Logger.LogWarning(
            "Database connection unavailable while applying migrations (attempt {Attempt}/{Max}): {Message}. Retrying in {Delay}s.",
            attempt, migrationMaxAttempts, ex.GetBaseException().Message, migrationRetryDelay.TotalSeconds);
        MySql.Data.MySqlClient.MySqlConnection.ClearAllPools();
        Thread.Sleep(migrationRetryDelay);
    }
}

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseSwagger(); 
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
{
    appBuilder.UseRequestAuthorization();
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Errores de conexión que conviene reintentar al arrancar: límite de conexiones del usuario (1226),
// demasiadas conexiones en el servidor (1040) o servidor inalcanzable (1042).
static bool IsTransientConnectionError(Exception ex)
{
    for (var current = ex; current is not null; current = current.InnerException)
    {
        if (current is MySql.Data.MySqlClient.MySqlException { Number: 1226 or 1040 or 1042 }) return true;
        if (current.Message.Contains("max_user_connections", StringComparison.OrdinalIgnoreCase)
            || current.Message.Contains("Too many connections", StringComparison.OrdinalIgnoreCase)
            || current.Message.Contains("Unable to connect", StringComparison.OrdinalIgnoreCase))
            return true;
    }
    return false;
}