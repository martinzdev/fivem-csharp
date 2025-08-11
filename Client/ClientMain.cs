using CitizenFX.Core;
using core.Shared.DependencyInjection;
using System.Reflection;
using core.Client.Utils;
using core.Shared;
using core.Shared.Attributes.onCommand;
using core.Shared.Attributes.onTick;

namespace core.Client
{
  public class ClientMain : BaseScript
  {
    private Container container;

    public ClientMain()
    {
      var logger = new Logger();
      LogHelper.LogAction = (message) => logger.Debug($"[DI] {message}");

      var builder = new ContainerBuilder();
      builder.RegisterType<onCommandRegistry>().SingleInstance();
      builder.RegisterType<onTickRegistry>().SingleInstance();
      builder.RegisterModule(Assembly.GetExecutingAssembly());
            
      container = builder.Build();
      container.ResolveControllers();
      
      logger.Debug("Client started!");
      
      var tickRegistry = container.Resolve<onTickRegistry>();
      tickRegistry.SetTickRegistrationHandler(tickHandler => this.Tick += tickHandler);
      
      SharedMain.Initialize(container);
    }
  }
}