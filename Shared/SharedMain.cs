using core.Shared.Utils;

namespace core.Shared
{
  public class SharedMain
  {
    public static void Main()
    {
      Application.StartRegistration();
      Application.AutoRegisterServices();
      Application.RegisterService<Logger, Logger>();
      Application.Build();

      var logger = Application.Get<Logger>();
      logger.Info("Application container running!");
    }
  }
}