using CitizenFX.Core;
using core.Shared.Attributes.onCommand;
using core.Shared.DependencyInjection;
using core.Shared.Logger;

namespace core.Client.Modules.Connection
{
  [Controller]
  public class Connection_Controller : BaseScript
  {
    private ILogger _logger;
    public Connection_Controller(ILogger logger)
    {
      _logger = logger;
      _logger.Info("Connection Controller started!");
    }

    [onCommand("dev:command", false)]
    public void onCommand()
    {
      Debug.WriteLine("Command executed!");
    }
  }
}