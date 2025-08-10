using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using core.Client.Services;

namespace core.Client.Modules.Player
{
    public class PlayerService : IPlayerService
    {
        private readonly ILogger _logger;

        public PlayerService(ILogger logger)
        {
            _logger = logger;
            _logger.Info("PlayerService initialized!");
        }

        public string GetPlayerName()
        {
            try
            {
                return "Unknown Player";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting player name: {ex.Message}");
                return "Unknown Player";
            }
        }

        public async Task TeleportTo(float x, float y, float z)
        {
            try
            {
                _logger.Info($"Teleporting player to: {x}, {y}, {z}");

                var playerPed = Game.PlayerPed;
                
                // Fade out
                API.DoScreenFadeOut(500);
                await BaseScript.Delay(500);

                // Teleport
                playerPed.Position = new Vector3(x, y, z);

                // Fade in
                API.DoScreenFadeIn(500);
                
                _logger.Info("Player teleported successfully!");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error teleporting player: {ex.Message}");
            }
        }

        public Task HealPlayer()
        {
            try
            {
                _logger.Info("Healing player...");

                var playerPed = Game.PlayerPed;
                playerPed.Health = playerPed.MaxHealth;
                playerPed.Armor = 100;

                _logger.Info("Player healed successfully!");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error healing player: {ex.Message}");
            }
            
            return Task.FromResult(0);
        }

        public bool IsPlayerInVehicle()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                return playerPed.CurrentVehicle != null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error checking if player is in vehicle: {ex.Message}");
                return false;
            }
        }
    }
}
