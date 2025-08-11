using core.Shared.Attributes.onCommand;
using core.Shared.DependencyInjection;
using core.Shared.Logger;

namespace core.Client.Modules.Connection
{
  [Controller]
  public class ConnectionController
  {
    private readonly ILogger _logger;
    public ConnectionController(ILogger logger)
    {
      _logger = logger;
      _logger.Info("Connection Controller started!");
    }

    [onCommand("dev:command")]
    public void onCommand()
    {
      _logger.Info("Command executed!");
    }
  }
}