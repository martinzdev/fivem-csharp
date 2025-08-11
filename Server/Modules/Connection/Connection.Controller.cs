using CitizenFX.Core;
using CitizenFX.Core.Native;
using core.Shared.Attributes.onCommand;
using core.Shared.DependencyInjection;
using core.Shared.Logger;

namespace core.Server.Modules.Connection
{
  [Controller]
  public class Connection_Controller
  {
    public ILogger _logger { get; }
    public Connection_Controller(ILogger logger)
    {
      _logger = logger;
      onStart();
    }

    public void onStart()
    {
      _logger.Info("Controller started!");
    }

    [onCommand("dev:command", false)]
    public void onCommand()
    {
      _logger.Info("Command executed!");
    }
  }
}