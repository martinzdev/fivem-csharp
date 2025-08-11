using System.Reflection;
using CitizenFX.Core;
using core.Shared;
using core.Shared.Attributes.onCommand;
using core.Shared.DependencyInjection;
using core.Shared.Logger;

namespace core.Server
{
    public class ServerMain : BaseScript
    {
        public Container container;
        public ServerMain()
        {
            LogHelper.LogAction = (message) => Debug.WriteLine($"[DI] {message}");

            var builder = new ContainerBuilder();
            builder.RegisterType<onCommandRegistry>().SingleInstance();
      
            builder.RegisterModule(Assembly.GetExecutingAssembly());
            
            container = builder.Build();
            container.ResolveControllers();
      
            var logger = container.Resolve<ILogger>();
            logger.Info("Server started!");
      
            SharedMain.Initialize(container);
        }
    }
}