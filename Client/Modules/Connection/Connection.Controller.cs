using core.Client.Utils.Logger;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.Connection
{
  [Controller]
  public class Connection_Controller
  {
    public Connection_Controller(ILogger logger)
    {
      logger.Info("Connection Controller started!");
    }
  }
}