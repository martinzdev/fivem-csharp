# 📦 Dependency Injection Container

A lightweight and powerful container for dependency management with support for multiple lifetimes and auto-registration via attributes.

## 🚀 Initial Setup

```csharp
// On application startup
Application.StartRegistration();
Application.AutoRegisterServices(); // Auto-registration via attributes
Application.Build();
```

## 🏷️ Service Lifetimes

| Lifetime | Description | Ideal Usage |
|----------|-------------|-------------|
| **Singleton** | One instance for the entire application | Logger, Configuration, Cache |
| **Scoped** | One instance per scope/operation | Database Context, Unit of Work |
| **Transient** | New instance on every resolution | Helpers, Utilities |

## 📝 Service Registration

### Via Attributes (Recommended)

```csharp
// Singleton
[Service(typeof(ILogger))]
[Singleton]
public class Logger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
}

// Scoped
[Service(typeof(IUserRepository), ServiceLifetime.Scoped)]
public class UserRepository : IUserRepository
{
    private readonly ILogger _logger;
    
    // Automatic dependency injection
    public UserRepository(ILogger logger)
    {
        _logger = logger;
    }
}

// Transient (default)
[Service(typeof(IEmailService))]
[Transient]
public class EmailService : IEmailService { }
```

### Via Manual Code

```csharp
Application.StartRegistration();

// Singleton
Application.RegisterSingleton<ILogger, Logger>();

// Scoped
Application.RegisterScoped<IUserRepository, UserRepository>();

// Transient
Application.RegisterTransient<IEmailService, EmailService>();

// Specific instance
var config = new AppConfig();
Application.RegisterInstance<IAppConfig>(config);

// Custom factory
Application.RegisterFactory<IDbConnection>(scope => 
    new SqlConnection(connectionString), ServiceLifetime.Scoped);

Application.Build();
```

## 🔧 Service Resolution

### Scoped Usage (Recommended)

```csharp
// For operations that need shared context
Application.WithScope(scope =>
{
    var userService = scope.Get<IUserService>();
    var auditService = scope.Get<IAuditService>();
    
    userService.CreateUser("John");
    auditService.LogAction("User created");
    // Both share the same scope/context
});

// With return value
var result = Application.WithScope(scope =>
{
    var userService = scope.Get<IUserService>();
    return userService.ValidateUser("John");
});
```

### Direct Usage (For simple cases)

```csharp
// Creates temporary scope automatically
var logger = Application.Get<ILogger>();
logger.Log("Hello World");
```

### Manual Scope

```csharp
using (var scope = Application.CreateScope())
{
    var service1 = scope.Get<IService1>();
    var service2 = scope.Get<IService2>();
    
    // Both share the same scope
    // Automatic dispose when exiting using
}
```

## 🎮 FiveM Practical Examples

### Gamemode Initial Setup

```csharp
public class ServerMain
{
    public ServerMain()
    {
        Application.StartRegistration();
        Application.AutoRegisterServices();
        
        // Server-specific registrations
        Application.RegisterSingleton<IConfig, ServerConfig>();
        Application.RegisterScoped<IDatabase, MySqlDatabase>();
        
        Application.Build();
        
        var logger = Application.Get<ILogger>();
        logger.Log("FiveM Server initialized successfully!");
    }
}
```

### Player Commands

```csharp
[Command("transfer")]
public void TransferMoney([FromSource] Player source, int targetId, int amount)
{
    // Each command = new scope with isolated context
    Application.WithScope(scope =>
    {
        var bankService = scope.Get<IBankService>();
        var playerService = scope.Get<IPlayerService>();
        var auditService = scope.Get<IAuditService>();
        
        var sourcePlayer = playerService.GetPlayer(source.Handle);
        var targetPlayer = playerService.GetPlayerById(targetId);
        
        if (bankService.TransferMoney(sourcePlayer, targetPlayer, amount))
        {
            auditService.LogTransaction($"{sourcePlayer.Name} transferred ${amount} to {targetPlayer.Name}");
            source.TriggerEvent("transferSuccess", amount, targetPlayer.Name);
        }
        else
        {
            source.TriggerEvent("transferFailed", "Insufficient funds");
        }
    });
}

[Command("veh")]
public void SpawnVehicle([FromSource] Player source, string model)
{
    Application.WithScope(scope =>
    {
        var vehicleService = scope.Get<IVehicleService>();
        var permissionService = scope.Get<IPermissionService>();
        
        if (!permissionService.HasPermission(source.Handle, "admin.vehicle"))
        {
            source.TriggerEvent("chatMessage", "System", "{FF0000}", "No permission!");
            return;
        }
        
        var vehicle = vehicleService.SpawnVehicle(model, source.Character.Position);
        vehicleService.SetVehicleOwner(vehicle, source.Handle);
    });
}
```

### Server Events

```csharp
[EventHandler("playerConnecting")]
public void OnPlayerConnecting([FromSource] Player source, string name, dynamic setKickReason, dynamic deferrals)
{
    Application.WithScope(scope =>
    {
        var playerService = scope.Get<IPlayerService>();
        var banService = scope.Get<IBanService>();
        var database = scope.Get<IDatabase>();
        
        deferrals.defer();
        
        if (banService.IsPlayerBanned(source.Identifiers["steam"]))
        {
            deferrals.done("You are banned from this server");
            return;
        }
        
        var playerData = playerService.LoadOrCreatePlayer(source.Identifiers["steam"], name);
        deferrals.done();
    });
}

[EventHandler("playerDropped")]
public void OnPlayerDropped([FromSource] Player source, string reason)
{
    Application.WithScope(scope =>
    {
        var playerService = scope.Get<IPlayerService>();
        var vehicleService = scope.Get<IVehicleService>();
        var logger = scope.Get<ILogger>();
        
        logger.Log($"{source.Name} disconnected: {reason}");
        
        playerService.SavePlayerData(source.Handle);
        vehicleService.RemovePlayerVehicles(source.Handle);
    });
}
```

### Job System

```csharp
[Command("startwork")]
public void StartWork([FromSource] Player source, string jobName)
{
    Application.WithScope(scope =>
    {
        var jobService = scope.Get<IJobService>();
        var playerService = scope.Get<IPlayerService>();
        var notificationService = scope.Get<INotificationService>();
        
        var player = playerService.GetPlayer(source.Handle);
        
        if (jobService.StartJob(player, jobName))
        {
            notificationService.SendSuccess(source, $"Started working as {jobName}");
            source.TriggerEvent("jobStarted", jobName);
        }
        else
        {
            notificationService.SendError(source, "Failed to start job");
        }
    });
}
```

## 🔍 Defining Services for FiveM

### Typical Gamemode Services

```csharp
// Player Service
public interface IPlayerService
{
    Player GetPlayer(string handle);
    PlayerData LoadOrCreatePlayer(string steamId, string name);
    void SavePlayerData(string handle);
}

[Service(typeof(IPlayerService), ServiceLifetime.Scoped)]
public class PlayerService : IPlayerService
{
    private readonly IDatabase _database;
    private readonly ILogger _logger;

    public PlayerService(IDatabase database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }

    public PlayerData LoadOrCreatePlayer(string steamId, string name)
    {
        _logger.Log($"Loading player data for {name}");
        
        var data = _database.GetPlayerData(steamId);
        if (data == null)
        {
            data = CreateNewPlayer(steamId, name);
            _database.SavePlayerData(data);
        }
        
        return data;
    }
    
    // ... other methods
}

// Vehicle Service
public interface IVehicleService
{
    Vehicle SpawnVehicle(string model, Vector3 position);
    void SetVehicleOwner(Vehicle vehicle, string playerId);
    void RemovePlayerVehicles(string playerId);
}

[Service(typeof(IVehicleService), ServiceLifetime.Scoped)]
public class VehicleService : IVehicleService
{
    private readonly IDatabase _database;
    private readonly ILogger _logger;

    public VehicleService(IDatabase database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }

    public Vehicle SpawnVehicle(string model, Vector3 position)
    {
        var vehicle = CitizenFX.Core.World.CreateVehicle(model, position);
        _logger.Log($"Spawned vehicle {model} at {position}");
        return vehicle;
    }
    
    // ... other methods
}
```

## ⚡ Best Practices

### ✅ Do

- Use **Scoped** for services that need to share context in one operation
- Use **Singleton** for expensive and stateless services (Logger, Config)
- Use **Transient** for lightweight and stateless services
- Prefer `WithScope()` for complete operations
- Register interfaces, not concrete implementations

### ❌ Avoid

- Resolving services outside scopes for Scoped services
- Storing references to Transient services
- Creating unnecessary scopes for simple operations

## 🔧 Troubleshooting

### Error: "Service not registered"
```csharp
// Make sure to register the service
[Service(typeof(IMyService))] // ✅ Correct
public class MyService : IMyService { }

// Or manually
Application.RegisterScoped<IMyService, MyService>();
```

### Error: "Application not built"
```csharp
// Always call Build() after registrations
Application.StartRegistration();
Application.AutoRegisterServices();
Application.Build(); // ✅ Required
```

### Error: "Cannot resolve scoped service outside of scope"
```csharp
// ❌ Wrong - trying to resolve Scoped outside scope
var service = Application.Get<IScopedService>();

// ✅ Correct - using scope
Application.WithScope(scope => 
{
    var service = scope.Get<IScopedService>();
});
```

---

**💡 Tip:** Use IntelliSense to discover all available methods. The container offers full flexibility while maintaining ease of use!