# Core for CitizenFX

This repository contains the **core**, a professional boilerplate for developing C# resources for FiveM using the CitizenFX platform. The goal of this project is to provide a solid, organized, and easily maintainable base to speed up the development of client and server scripts.

## Project Structure

To edit the project, open the `core.sln` solution in Visual Studio or your preferred editor. The structure is separated into three main projects:

- **core.Client**: Client-side scripts and logic.
- **core.Server**: Server-side scripts and logic.
- **core.Shared**: Code shared between client and server, including the Dependency Injection system.

## Dependency Injection System

This project includes a custom-built Dependency Injection (DI) container located in `core.Shared.DependencyInjection`. The DI system provides:

- **Constructor Injection**: Automatic dependency resolution through constructors
- **Singleton Management**: Cached single instances for stateless services
- **Interface Mapping**: Register implementations against multiple interfaces
- **Factory Methods**: Custom instance creation logic
- **Array Injection**: Resolve multiple implementations of the same interface
- **Comprehensive Logging**: Debug DI operations with configurable logging
- **Attribute-Based Registration**: Automatic service and controller registration using decorators
- **Module Registration**: Bulk registration of all services and controllers in an assembly

### Quick DI Usage Example

```csharp
// Setup in ClientMain.cs or ServerMain.cs
var builder = new ContainerBuilder();

// Register all services and controllers automatically
builder.RegisterModule(Assembly.GetExecutingAssembly());

// Build and resolve
var container = builder.Build();
container.ResolveControllers(); // Automatically resolve all controllers
```

### DI Architecture Components

- **Container**: Main service resolution engine
- **ContainerBuilder**: Fluent API for service registration
- **Registration System**: Service registration metadata
- **Extensions**: Fluent configuration methods (`.SingleInstance()`, `.As<T>()`)
- **LogHelper**: Configurable logging for debugging DI operations
- **Module Attributes**: `[Service]` and `[Controller]` decorators for automatic registration
- **ContainerBuilder Extensions**: `RegisterModule()` and `ResolveControllers()` methods

For complete DI documentation, see the [Dependency Injection Guide](#dependency-injection-detailed-guide) below.

## Building

You can build the projects in two ways: using the `build.cmd` script or directly through your preferred IDE, such as Visual Studio or JetBrains Rider (IDEA).

### Option 1: Build via Script

1. Open the Command Prompt in the root of the repository.
2. Run the command:

   ```
   build.cmd
   ```

The script will automatically build the required projects and generate the files ready for use in FiveM.

**After building, a `bin` folder will be generated containing the compiled client and server `.dll` files.**

### Option 2: Build via IDE

You can also open the `core.sln` solution in any .NET-compatible IDE, such as Visual Studio or JetBrains Rider (IDEA), and build the projects directly there using the IDE's build commands.

**Similarly, when building via the IDE, a `bin` folder will be created with the compiled `.dll` files for both client and server.**

---

# Dependency Injection Detailed Guide

This section provides comprehensive documentation for the custom Dependency Injection (DI) container implemented in the `core.Shared.DependencyInjection` namespace.

## Overview

The DI system provides a lightweight, custom-built container for managing service registration and resolution, specifically designed for FiveM environments. It supports constructor injection, singleton instances, interface-to-implementation mapping, factory methods, and **attribute-based automatic registration** using decorators.

## Architecture

### Core Components

#### 1. Container (`Container.cs`)
The main DI container responsible for service resolution and instance management.

**Key Features:**
- Constructor injection with automatic dependency resolution
- Singleton instance management with caching
- Array injection for multiple service implementations
- Factory method support
- Comprehensive logging for debugging

**Important Methods:**
- `Resolve<T>()`: Resolves a service by generic type
- `Resolve(Type type)`: Resolves a service by Type object
- `ResolveSingleInstance()`: Handles singleton lifetime management
- `ResolveArray()`: Resolves arrays of services implementing the same interface
- `GetRegisteredTypes()`: Returns all registered types for inspection

#### 2. ContainerBuilder (`ContainerBuilder.cs`)
Fluent API for registering services and building the container.

**Registration Methods:**
- `RegisterType<T>()`: Register a concrete type
- `Register<T>(Func<T> factory)`: Register with factory method
- `Build()`: Create the container instance

#### 3. Registration System (`Registration.cs`)
Base classes for service registration metadata.

**Classes:**
- `Registration<T>`: Generic registration for type-safe registration
- `Registration`: Base abstract class containing registration metadata

**Properties:**
- `InstanceType`: The concrete implementation type
- `Types`: List of types this registration can resolve (including interfaces)
- `SingleInstance`: Boolean flag for singleton lifetime
- `Factory`: Optional factory function for instance creation

#### 4. Registration Extensions (`RegistrationExtensions.cs`)
Fluent extension methods for configuring registrations.

**Methods:**
- `.SingleInstance()`: Configure as singleton
- `.As<T>()`: Register as additional interface/base class

#### 5. LogHelper (`LogHelper.cs`)
Configurable logging system for DI operations.

**Usage:**
```csharp
LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");
```

#### 6. Module Attributes (`ModuleAttributes.cs`) ⭐ **NEW**
Attribute-based decorators for automatic service and controller registration.

**Attributes:**
- `[Service]`: Marks a class as a service for automatic registration
- `[Controller]`: Marks a class as a controller for automatic registration
- `Lifecycle`: Enum for controlling service lifetime (Singleton/Transient)

#### 7. ContainerBuilder Extensions (`ContainerBuilderExtensions.cs`) ⭐ **NEW**
Extension methods for bulk registration and controller resolution.

**Methods:**
- `RegisterModule(Assembly)`: Automatically registers all services and controllers in an assembly
- `ResolveControllers()`: Automatically resolves all registered controllers

## Usage Examples

### Modern Setup with Decorators ⭐ **NEW**

#### Client-Side Setup (ClientMain.cs)
```csharp
public class ClientMain : BaseScript
{
    private Container container;

    public ClientMain()
    {
        LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");

        var builder = new ContainerBuilder();
        
        // Automatically register all services and controllers
        builder.RegisterModule(Assembly.GetExecutingAssembly());
            
        container = builder.Build();
        
        // Automatically resolve all controllers
        container.ResolveControllers();
        
        var logger = container.Resolve<ILogger>();
        logger.Info("Client started!");
    }
}
```

#### Server-Side Setup (ServerMain.cs)
```csharp
public class ServerMain : BaseScript
{
    public Container container;
    
    public ServerMain()
    {
        LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");

        var builder = new ContainerBuilder();
        
        // Automatically register all services and controllers
        builder.RegisterModule(Assembly.GetExecutingAssembly());
        
        container = builder.Build();
        
        // Automatically resolve all controllers
        container.ResolveControllers();
        
        var logger = container.Resolve<ILogger>();
        logger.Info("Server started!");
    }
}
```

### Service Registration with Decorators ⭐ **NEW**

#### Basic Service Registration
```csharp
[Service] // Default: Singleton lifecycle
public class Logger : ILogger
{
    public void Info(string message) => Debug.WriteLine($"[INFO] {message}");
    public void Error(string message) => Debug.WriteLine($"[ERROR] {message}");
}
```

#### Service with Custom Lifecycle
```csharp
[Service(Lifecycle.Transient)] // New instance each time
public class RequestHandler : IRequestHandler
{
    // Implementation...
}
```

#### Service with Interface Mapping
```csharp
[Service(Lifecycle.Singleton, typeof(ILogger), typeof(INotificationService))]
public class Logger : ILogger, INotificationService
{
    // Implementation for both interfaces...
}
```

#### Service with Multiple Interfaces
```csharp
[Service(Lifecycle.Singleton, typeof(IEmailService), typeof(INotificationService))]
public class EmailService : IEmailService, INotificationService
{
    // Same instance will be used for both interfaces
}
```

### Controller Registration with Decorators ⭐ **NEW**

#### Basic Controller Registration
```csharp
[Controller] // Default: Singleton lifecycle
public class PlayerController : BaseScript
{
    private readonly ILogger _logger;
    private readonly IPlayerService _playerService;

    public PlayerController(ILogger logger, IPlayerService playerService)
    {
        _logger = logger;
        _playerService = playerService;
        _logger.Info("PlayerController initialized!");
    }
}
```

#### Controller with Custom Lifecycle
```csharp
[Controller(Lifecycle.Transient)] // New instance each time
public class SessionController : BaseScript
{
    // Implementation...
}
```

### Traditional Manual Registration (Still Supported)

#### Manual Service Registration
```csharp
var builder = new ContainerBuilder();

// Register services manually
builder.RegisterType<Logger>()
       .SingleInstance()
       .As<ILogger>();

builder.RegisterType<PlayerService>()
       .SingleInstance()
       .As<IPlayerService>();

var container = builder.Build();
```

#### Manual Controller Registration
```csharp
var builder = new ContainerBuilder();

// Register controllers manually
builder.RegisterType<PlayerController>()
       .SingleInstance();

builder.RegisterType<VehicleController>()
       .SingleInstance();

var container = builder.Build();
```

### Constructor Injection Example

Controllers automatically receive their dependencies through constructor injection:

```csharp
[Controller]
public class PlayerController : BaseScript
{
    private readonly ILogger _logger;
    private readonly IPlayerService _playerService;
    private readonly IVehicleService _vehicleService;

    public PlayerController(ILogger logger, IPlayerService playerService, IVehicleService vehicleService)
    {
        _logger = logger;
        _playerService = playerService;
        _vehicleService = vehicleService;
        _logger.Info("PlayerController initialized!");
    }
}
```

### Factory Registration Example

```csharp
builder.Register<IComplexService>(() => new ComplexService(connectionString))
       .SingleInstance();
```

### Array Injection Example

For multiple implementations of the same interface:

```csharp
// Register multiple handlers
builder.RegisterType<Handler1>().As<IHandler>();
builder.RegisterType<Handler2>().As<IHandler>();

// Constructor will receive IHandler[] array
public class MultiHandlerService
{
    public MultiHandlerService(IHandler[] handlers) { ... }
}
```

## Registration Patterns

### 1. Attribute-Based Registration ⭐ **NEW**

#### Service Attributes
```csharp
[Service] // Singleton, auto-register as implemented interfaces
[Service(Lifecycle.Transient)] // Transient lifecycle
[Service(Lifecycle.Singleton, typeof(ILogger), typeof(INotificationService)] // Custom interfaces
```

#### Controller Attributes
```csharp
[Controller] // Singleton, auto-register
[Controller(Lifecycle.Transient)] // Transient lifecycle
```

### 2. Singleton Services
```csharp
// Using decorator
[Service(Lifecycle.Singleton)]
public class DatabaseService : IDatabaseService { }

// Manual registration
builder.RegisterType<DatabaseService>()
       .SingleInstance()
       .As<IDatabaseService>();
```

### 3. Transient Services
```csharp
// Using decorator
[Service(Lifecycle.Transient)]
public class RequestHandler : IRequestHandler { }

// Manual registration
builder.RegisterType<RequestHandler>()
       .As<IRequestHandler>(); // New instance each time
```

### 4. Factory Registration
```csharp
builder.Register<IConfigService>(() => new ConfigService(Environment.GetEnvironmentVariable("CONFIG_PATH")))
       .SingleInstance();
```

### 5. Interface Mapping
```csharp
// Using decorator
[Service(Lifecycle.Singleton, typeof(IEmailService), typeof(INotificationService))]
public class EmailService : IEmailService, INotificationService { }

// Manual registration
builder.RegisterType<EmailService>()
       .As<IEmailService>()
       .As<INotificationService>(); // Same instance for both interfaces
```

## Lifetime Management

### Singleton Lifetime
- Services registered with `.SingleInstance()` or `[Service(Lifecycle.Singleton)]` are created once and cached
- Subsequent resolutions return the same instance
- Cache key: `Type.AssemblyQualifiedName`

### Transient Lifetime
- Default lifetime when `.SingleInstance()` is not called or `[Service(Lifecycle.Transient)]` is used
- New instance created for each resolution
- No caching performed

## Error Handling

### Common Exceptions

1. **Type Already Registered**
   ```
   Exception: Type {type.AssemblyQualifiedName} already registered.
   ```
   **Solution**: Avoid duplicate registrations or use different interface mappings

2. **Cannot Resolve Type**
   ```
   Exception: Could not resolve type {type.AssemblyQualifiedName}
   ```
   **Solution**: Ensure the type is registered before resolving

3. **Constructor Dependencies**
   - The container automatically resolves constructor parameters
   - All constructor dependencies must be registered
   - Circular dependencies will cause stack overflow

## Debugging and Logging

### Enable DI Logging
```csharp
LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");
```

### Log Messages
- `"Create instance: {Type}"` - When creating new instances
- `"Resolve constructor parameter: {ParameterType}"` - During constructor injection
- `"Resolve array: {ArrayType}"` - When resolving arrays

## Best Practices

### 1. Use Attribute-Based Registration ⭐ **NEW**
```csharp
// Good: Use decorators for automatic registration
[Service(Lifecycle.Singleton)]
public class Logger : ILogger { }

[Controller]
public class PlayerController : BaseScript { }

// Then use RegisterModule for bulk registration
builder.RegisterModule(Assembly.GetExecutingAssembly());
```

### 2. Register Dependencies Before Dependents
```csharp
// Register dependencies first
builder.RegisterType<Logger>().As<ILogger>();
builder.RegisterType<DatabaseService>().As<IDatabaseService>();

// Then register services that depend on them
builder.RegisterType<UserService>().As<IUserService>();
```

### 3. Use Interface Abstractions
```csharp
// Good: Register against interface
builder.RegisterType<FileLogger>().As<ILogger>();

// Avoid: Registering concrete types directly (unless necessary)
builder.RegisterType<FileLogger>();
```

### 4. Prefer Constructor Injection
```csharp
// Good: Dependencies declared in constructor
public class UserService
{
    public UserService(ILogger logger, IDatabaseService database) { ... }
}
```

### 5. Use Singletons for Stateless Services
```csharp
// Stateless services should be singletons
[Service(Lifecycle.Singleton)]
public class EmailService : IEmailService { }

// Stateful services should be transient
[Service(Lifecycle.Transient)]
public class UserSession : IUserSession { }
```

### 6. Configure Logging Early
```csharp
// Configure logging before building container
LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");
var builder = new ContainerBuilder();
```

### 7. Use RegisterModule for Clean Setup ⭐ **NEW**
```csharp
// Clean and simple setup
var builder = new ContainerBuilder();
builder.RegisterModule(Assembly.GetExecutingAssembly());
var container = builder.Build();
container.ResolveControllers();
```

## Integration with FiveM

### BaseScript Integration
Both `ClientMain` and `ServerMain` inherit from `BaseScript` and set up DI in their constructors:

```csharp
public class ClientMain : BaseScript
{
    private Container _container;

    public ClientMain()
    {
        SetupDependencyInjection();
        // Controllers are automatically resolved
    }
}
```

### Command Registration
Controllers can register FiveM commands and use injected dependencies:

```csharp
[Controller]
public class PlayerController : BaseScript
{
    private readonly ILogger _logger;
    private readonly IPlayerService _playerService;

    public PlayerController(ILogger logger, IPlayerService playerService)
    {
        _logger = logger;
        _playerService = playerService;
        
        API.RegisterCommand("heal", new Action<int, List<object>, string>((source, args, raw) =>
        {
            HealCommand();
        }), false);
    }
}
```

## Limitations

1. **No Circular Dependencies**: The container cannot resolve circular dependencies
2. **Single Constructor**: Only supports single constructor per type
3. **No Property Injection**: Only constructor injection is supported
4. **No Conditional Registration**: No built-in support for conditional registrations
5. **~~No Decorator Pattern~~**: ✅ **NEW**: Decorator pattern is now supported through `[Service]` and `[Controller]` attributes

## File Structure
```
Shared/DependencyInjection/
├── Container.cs                    # Main DI container
├── ContainerBuilder.cs             # Fluent registration API
├── Registration.cs                 # Registration metadata classes
├── RegistrationExtensions.cs       # Fluent extension methods
├── LogHelper.cs                    # Configurable logging
├── ModuleAttributes.cs             # ⭐ NEW: Service and Controller decorators
└── ContainerBuilderExtensions.cs   # ⭐ NEW: Module registration extensions
```

## Migration from Manual Registration

If you're migrating from manual registration to attribute-based registration:

### Before (Manual)
```csharp
var builder = new ContainerBuilder();

builder.RegisterType<Logger>()
       .SingleInstance()
       .As<ILogger>();

builder.RegisterType<PlayerController>()
       .SingleInstance();

var container = builder.Build();
```

### After (Attribute-Based) ⭐ **NEW**
```csharp
// Add decorators to your classes
[Service(Lifecycle.Singleton)]
public class Logger : ILogger { }

[Controller]
public class PlayerController : BaseScript { }

// Then use automatic registration
var builder = new ContainerBuilder();
builder.RegisterModule(Assembly.GetExecutingAssembly());
var container = builder.Build();
container.ResolveControllers();
```

This DI system provides a solid foundation for dependency management in FiveM C# resources while maintaining simplicity and performance. The new attribute-based system makes it even easier to manage dependencies with minimal boilerplate code.

---

If you have any questions or need support, please refer to the official FiveM documentation or open an issue in this repository.
