using CitizenFX.Core.Native;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.Player.Services
{
  [Service(Lifecycle.Transient)]
  public class PlayerHealthService
  {
    public void SetPlayerInvincible(bool invincible)
    {
      var ped = API.PlayerPedId();
      API.SetEntityInvincible(ped, invincible);
      API.SetPlayerInvincible(API.PlayerId(), invincible);
      API.SetEntityCanBeDamaged(ped, !invincible);
    }
  }
}