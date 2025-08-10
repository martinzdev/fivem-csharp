using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using core.Client.Services;

namespace core.Client.Modules.Vehicle
{
    public class VehicleService : IVehicleService
    {
        private readonly ILogger _logger;

        public VehicleService(ILogger logger)
        {
            _logger = logger;
            _logger.Info("VehicleService initialized!");
        }

        public async Task SpawnVehicle(string modelName)
        {
            try
            {
                _logger.Info($"Attempting to spawn vehicle: {modelName}");

                uint modelHash = (uint)API.GetHashKey(modelName);
                
                if (!API.IsModelInCdimage(modelHash) || !API.IsModelAVehicle(modelHash))
                {
                    _logger.Error($"Invalid vehicle model: {modelName}");
                    return;
                }

                API.RequestModel(modelHash);

                int timeout = 0;
                while (!API.HasModelLoaded(modelHash) && timeout < 100)
                {
                    await BaseScript.Delay(100);
                    timeout++;
                }

                if (!API.HasModelLoaded(modelHash))
                {
                    _logger.Error($"Failed to load model: {modelName}");
                    return;
                }

                var playerPed = Game.PlayerPed;
                var pos = playerPed.Position;
                var heading = playerPed.Heading;

                int vehicle = API.CreateVehicle(modelHash, pos.X, pos.Y, pos.Z, heading, true, false);

                if (vehicle != 0)
                {
                    API.SetPedIntoVehicle(playerPed.Handle, vehicle, -1);
                    _logger.Info($"Vehicle '{modelName}' spawned successfully!");
                }
                else
                {
                    _logger.Error($"Failed to spawn vehicle: {modelName}");
                }

                API.SetModelAsNoLongerNeeded(modelHash);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error spawning vehicle: {ex.Message}");
            }
        }

        public Task DeleteVehicle()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                var vehicle = playerPed.CurrentVehicle;

                if (vehicle != null)
                {
                    vehicle.Delete();
                    _logger.Info("Current vehicle deleted!");
                }
                else
                {
                    _logger.Warning("No vehicle to delete!");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting vehicle: {ex.Message}");
            }
            
            return Task.FromResult(0);
        }

        public bool IsVehicleValid(string modelName)
        {
            try
            {
                uint modelHash = (uint)API.GetHashKey(modelName);
                return API.IsModelInCdimage(modelHash) && API.IsModelAVehicle(modelHash);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error checking vehicle validity: {ex.Message}");
                return false;
            }
        }
    }
}
