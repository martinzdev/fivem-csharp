using CitizenFX.Core;
using core.Shared;
using core.Shared.Utils;

namespace core.Server
{
    public class ServerMain : BaseScript
    {
        private Logger logger;
        public ServerMain()
        {
            SharedMain.Main();
            
            this.logger = Application.Get<Logger>();
        }

        [Command("hello_server")]
        public void HelloServer()
        {
            logger.Info("Sure, hello.");
        }
    }
}