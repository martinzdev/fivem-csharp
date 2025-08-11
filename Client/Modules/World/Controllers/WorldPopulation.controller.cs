using System.Threading.Tasks;
using core.Client.Modules.World.Services;
using core.Shared.Attributes.onTick;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.World.Controllers
{
  [Controller]
  public class WorldPopulationController
  {
    private readonly WorldPopulationService _worldPopulationService;

    public WorldPopulationController(WorldPopulationService worldPopulationService)
    {
      _worldPopulationService = worldPopulationService;
    }
    
    [onTick]
    public async Task onTick()
    {
      _worldPopulationService.DisableWantedPolice();
      _worldPopulationService.DisablePedPopulation();
      _worldPopulationService.DisaleVehiclePopulation();
      _worldPopulationService.PoliceIgnorePlayer();
      
      await Task.FromResult(0);
    }
  }
}