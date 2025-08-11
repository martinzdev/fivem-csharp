using CitizenFX.Core.Native;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.Player.Services
{
  [Service(Lifecycle.Transient)]
  public class PlayerCombatService
  {
    public void DisableWeapons()
    {
      var ped = API.PlayerPedId();
      API.SetPedCanSwitchWeapon(ped, false);
      API.SetCurrentPedWeapon(ped, (uint)API.GetHashKey("WEAPON_UNARMED"), true);
    }

    public void DisableCombatControls()
    {
      API.DisableControlAction(0, 24, true); // Primary attack
      API.DisableControlAction(0, 25, true); // Aim
      API.DisableControlAction(0, 68, true); // Alternate attack
      API.DisableControlAction(0, 69, true); // Vehicle weapon fire
      API.DisableControlAction(0, 70, true); // Vehicle melee attack
      API.DisableControlAction(0, 91, true); // Aim zoom
    }

    public void DisableMovementExtras()
    {
      API.DisableControlAction(0, 36, true); // CTRL - crouch
      API.DisableControlAction(0, 44, true); // Q (alt)
      API.DisableControlAction(0, 45, true); // Cover
    }

    public void DisableCoverAndActionMode()
    {
      var ped = API.PlayerPedId();
      API.SetPlayerCanUseCover(ped, false);
      API.SetPedUsingActionMode(ped, false, -1, "DEFAULT_ACTION");
    }

  }
}