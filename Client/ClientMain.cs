using CitizenFX.Core;
using core.Shared;
using core.Shared.Utils;

namespace core.Client
{
  public class ClientMain : BaseScript
  {
    private Logger logger;
    public ClientMain()
    {
      SharedMain.Main();

      this.logger = Application.Get<Logger>();
    }

    [Command("hello_client")]
    public void HelloClient()
    {
      logger.Info("Sure, hello.");
    }
  }
}