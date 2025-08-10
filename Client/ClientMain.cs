using CitizenFX.Core;
using core.Shared.DependencyInjection;
using System.Reflection;
using core.Client.Utils.Logger;

namespace core.Client
{
  public class ClientMain : BaseScript
  {
    private Container container;

    public ClientMain()
    {
      LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");

      var builder = new ContainerBuilder();
      builder.RegisterModule(Assembly.GetExecutingAssembly());
            
      container = builder.Build();
      container.ResolveControllers();
      
      var logger = container.Resolve<ILogger>();
      logger.Info("Server started!");
    }
  }
}