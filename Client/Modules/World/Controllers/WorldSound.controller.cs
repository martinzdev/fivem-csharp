using CitizenFX.Core.Native;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.World.Controllers
{
  [Controller]
  public class WorldSound_controller
  {
    public WorldSound_controller()
    {
      API.StartAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
    }
  }
}