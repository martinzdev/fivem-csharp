using System.Reflection;
using CitizenFX.Core;
using core.Shared;
using core.Shared.Attributes.onCommand;
using core.Shared.Attributes.onTick;
using core.Shared.DependencyInjection;
using core.Shared.Logger;

namespace core.Server
{
    public class ServerMain : BaseScript
    {
        public Container container;
        public ServerMain()
        {
            var logger = new Logger();
            
            LogHelper.LogAction = (message) => logger.Debug($"[DI] {message}");

            var builder = new ContainerBuilder();
            builder.RegisterType<onCommandRegistry>().SingleInstance();
            builder.RegisterType<onTickRegistry>().SingleInstance();
      
            builder.RegisterModule(Assembly.GetExecutingAssembly());
            
            container = builder.Build();
            container.ResolveControllers();
      
            logger.Debug("Server started!");
      
            // Set this ServerMain as the master script for the tick registry
            var tickRegistry = container.Resolve<onTickRegistry>();
            tickRegistry.SetTickRegistrationHandler(tickHandler => this.Tick += tickHandler);
            
            SharedMain.Initialize(container);
        }
    }
}