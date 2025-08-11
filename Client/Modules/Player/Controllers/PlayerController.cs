using System.Threading.Tasks;
using core.Client.Modules.Player.Services;
using core.Shared.Attributes.onTick;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.Player.Controllers
{
  [Controller]
  public class PlayerController
  {
    private readonly PlayerCombatService _playerCombatService;
    private readonly PlayerHealthService _playerHealthService;

    public PlayerController(PlayerCombatService playerCombatService, PlayerHealthService playerHealthService)
    {
      _playerCombatService = playerCombatService;
      _playerHealthService = playerHealthService;
    }

    [onTick]
    public async Task OnTick()
    {
      _playerCombatService.DisableWeapons();
      _playerCombatService.DisableCombatControls();
      _playerCombatService.DisableMovementExtras();
      _playerCombatService.DisableCoverAndActionMode();
      
      _playerHealthService.SetPlayerInvincible(true);

      await Task.FromResult(0);
    }
  }
}