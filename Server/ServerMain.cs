using CitizenFX.Core;
using core.Server.Modules.Connection;
using core.Server.Utils.Logger;
using core.Shared;
using core.Shared.DependencyInjection;

namespace core.Server
{
    public class ServerMain : BaseScript
    {
        public Container container;
        public ServerMain()
        {
            Debug.WriteLine("Server running!");
            
            var builder = new ContainerBuilder();
            
            builder.RegisterType<Logger>()
                .SingleInstance()
                .As<ILogger>();


            builder.RegisterType<Connection_Controller>();
            container = builder.Build();

            container.Resolve<Connection_Controller>();
        }
    }
}