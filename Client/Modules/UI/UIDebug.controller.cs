using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using core.Shared.Attributes.onCommand;
using core.Shared.Attributes.onTick;
using core.Shared.DependencyInjection;

namespace core.Client.Modules.UI
{
  [Controller]
  public class UIDebugController
  {
    private bool _debugEnabled;

    
    [onCommand("dev:debug", false)]
    public void ToggleDebug()
    {
      _debugEnabled = !_debugEnabled;
      Debug.WriteLine($"Debug: {_debugEnabled}");
    }
    
    [onTick]
    public async Task OnTick()
    {
      if (!_debugEnabled) return;
      var player = Game.Player;
      var ped = player.Character;
      var pos = ped.Position;

      string info = $"Posição do Jogador:\nX: {pos.X:F2}\nY: {pos.Y:F2}\nZ: {pos.Z:F2}\n";
      info += $"Vida: {ped.Health}\n";
      info += $"Armadura: {ped.Armor}\n";
      info += $"Velocidade: {ped.Velocity.Length():F2} m/s";

      DrawTextOnScreen(info, 0.015f, 0.015f, 0.4f, System.Drawing.Color.FromArgb(255,255,0,1));

      await Task.FromResult(0);
    }


    private void DrawTextOnScreen(string text, float x, float y, float scale, System.Drawing.Color color)
    {
      API.SetTextFont(0);
      API.SetTextProportional(true);
      API.SetTextScale(scale, scale);
      API.SetTextColour(color.R, color.G, color.B, color.A);
      API.SetTextDropshadow(0, 0, 0, 0, 255);
      API.SetTextEdge(1, 0, 0, 0, 255);
      API.SetTextDropShadow();
      API.SetTextOutline();
      API.SetTextEntry("STRING");
      API.AddTextComponentString(text);
      API.DrawText(x, y);
    }
  }
}