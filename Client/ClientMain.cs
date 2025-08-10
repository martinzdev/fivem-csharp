using CitizenFX.Core;
using core.Shared;
using core.Shared.DependencyInjection;
using core.Client.Services;
using core.Client.Modules.Vehicle;
using core.Client.Modules.Player;
using System;
using core.Client.Modules;

namespace core.Client
{
  public class ClientMain : BaseScript
  {
    private Container _container;

    public ClientMain()
    {
      SetupDependencyInjection();
      InitializeControllers();
      Debug.WriteLine("Client running with DI!");
    }

    private void SetupDependencyInjection()
    {
      LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");

      var builder = new ContainerBuilder();

      builder.RegisterType<Logger>()
             .SingleInstance()
             .As<ILogger>();

      builder.RegisterType<VehicleService>()
             .SingleInstance()
             .As<IVehicleService>();

      builder.RegisterType<PlayerService>()
             .SingleInstance()
             .As<IPlayerService>();

      builder.RegisterType<VehicleController>()
             .SingleInstance();

      builder.RegisterType<PlayerController>()
             .SingleInstance();

      _container = builder.Build();

      Debug.WriteLine("Dependency Injection container built successfully!");
    }

    private void InitializeControllers()
    {
      try
      {
        // Resolver e instanciar os controllers
        var vehicleController = _container.Resolve<VehicleController>();
        var playerController = _container.Resolve<PlayerController>();
        Debug.WriteLine("All controllers resolved and initialized!");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error initializing controllers: {ex.Message}");
      }
    }
  }
}