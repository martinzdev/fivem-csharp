using System.Reflection;
using CitizenFX.Core;
using core.Server.Utils.Logger;
using core.Shared.DependencyInjection;

namespace core.Server
{
    public class ServerMain : BaseScript
    {
        public Container container;
        public ServerMain()
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