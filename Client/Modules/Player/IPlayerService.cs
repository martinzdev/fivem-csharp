using System.Threading.Tasks;

namespace core.Client.Modules.Player
{
  public interface IPlayerService
  {
    string GetPlayerName();
    Task TeleportTo(float x, float y, float z);
    Task HealPlayer();
    bool IsPlayerInVehicle();
  }
}