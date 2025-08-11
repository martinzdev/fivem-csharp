using CitizenFX.Core;
using core.Shared.DependencyInjection;
using System.Reflection;
using core.Shared;
using core.Shared.Attributes.onCommand;
using core.Shared.Logger;

namespace core.Client
{
  public class ClientMain : BaseScript
  {
    private Container container;

    public ClientMain()
    {
      LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");

      var builder = new ContainerBuilder();
      builder.RegisterType<onCommandRegistry>().SingleInstance();
      
      builder.RegisterModule(Assembly.GetExecutingAssembly());
            
      container = builder.Build();
      container.ResolveControllers();
      
      var logger = container.Resolve<ILogger>();
      logger.Info("Client started!");
      
      SharedMain.Initialize(container);
    }
  }
}