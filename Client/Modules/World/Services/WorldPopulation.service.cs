using CitizenFX.Core.Native;

using core.Shared.DependencyInjection;

namespace core.Client.Modules.World.Services
{
  [Service(Lifecycle.Transient)]
  public class WorldPopulationService
  {
    public void DisableWantedPolice()
    {
      var ped = API.PlayerPedId();
      
      for (int index = 0; index < 12; ++index) API.EnableDispatchService(index, false);
      
      API.SetPlayerWantedLevel(ped, 0, false);
      API.SetPlayerWantedLevelNow(ped, false);
      API.SetPlayerWantedLevelNoDrop(ped, 0, false);
    }

    public void DisablePedPopulation()
    {
      API.SetPedPopulationBudget(0);
      API.SetPedDensityMultiplierThisFrame(0);
      API.SetScenarioPedDensityMultiplierThisFrame(0, 0);
    }
    
    public void DisaleVehiclePopulation()
    {
      API.SetPedPopulationBudget(0);
      API.SetVehicleDensityMultiplierThisFrame(0);
      API.SetRandomVehicleDensityMultiplierThisFrame(0);
      API.SetParkedVehicleDensityMultiplierThisFrame(0);
      
      var pos = API.GetEntityCoords(API.PlayerPedId(), true);
      float range = 500.0f;
      
      API.RemoveVehiclesFromGeneratorsInArea(
        pos.X - range, pos.Y - range, pos.Z - range,
        pos.X + range, pos.Y + range, pos.Z + range,
        0
      );
    }

    public void PoliceIgnorePlayer()
    {
      API.SetPoliceIgnorePlayer(API.PlayerPedId(), true);
      API.SetDispatchCopsForPlayer(API.PlayerPedId(), false);
    }
  }
}