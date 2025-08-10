using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using core.Client.Services;
namespace core.Client.Modules.Vehicle
{
  public class VehicleController
  {
    private readonly ILogger _logger;
    private readonly IVehicleService _vehicleService;
    
    public VehicleController(ILogger logger, IVehicleService vehicleService)
    {
      _logger = logger;
      _vehicleService = vehicleService;
      _logger.Info("VehicleController initialized!");
      
      API.RegisterCommand("veh", new Action<int, List<object>, string>((source, args, raw) =>
      {
        // Converte os args para string[]
        string[] strArgs = args.Select(a => a.ToString()).ToArray();
        SpawnVehicleCommand(strArgs);
      }), false);
    }
    
    public async void SpawnVehicleCommand(string[] args)
    {
      if (args.Length == 0)
      {
        _logger.Info("Uso: /veh <nome_do_modelo>");
        return;
      }

      string modelName = args[0];
      await _vehicleService.SpawnVehicle(modelName);
    }

    [Command("delcar")]
    public async void DeleteVehicleCommand(string[] args)
    {
      await _vehicleService.DeleteVehicle();
    }

    [Command("checkcar")]
    public void CheckVehicleCommand(string[] args)
    {
      if (args.Length == 0)
      {
        _logger.Info("Uso: /checkcar <nome_do_modelo>");
        return;
      }

      string modelName = args[0];
      bool isValid = _vehicleService.IsVehicleValid(modelName);
      
      if (isValid)
      {
        _logger.Info($"Modelo '{modelName}' é válido!");
      }
      else
      {
        _logger.Warning($"Modelo '{modelName}' é inválido!");
      }
    }
  }
}