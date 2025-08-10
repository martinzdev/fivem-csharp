using core.Server.Utils.Logger;

namespace core.Server.Modules.Connection
{
  public class Connection_Controller
  {
    public ILogger Logger { get; }
    public Connection_Controller(ILogger logger)
    {
      Logger = logger;
      onStart();
    }

    public void onStart()
    {
      Logger.Info("Controller started!");
    }
  }
}