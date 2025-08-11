using System.Linq;
using System.Reflection;
using core.Shared.Attributes.onCommand;
using core.Shared.DependencyInjection;

namespace core.Shared
{
  public class SharedMain
  {
    public static void Initialize(Container container)
    {
      var controllers = container.GetRegisteredTypes()
        .Where(t => t.GetCustomAttributes<ControllerAttribute>().Any())
        .Select(t => container.Resolve(t))
        .ToList();

      var commandRegistry = container.Resolve<onCommandRegistry>();
      commandRegistry.RegisterAllCommands(controllers);
    }
  }
}