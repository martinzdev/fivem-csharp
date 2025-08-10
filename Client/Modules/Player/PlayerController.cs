using CitizenFX.Core.Native;
using core.Client.Modules.Vehicle;
using core.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace core.Client.Modules.Player
{
  public class PlayerController
  {
    private readonly ILogger _logger;
    private readonly IPlayerService _playerService;
    private readonly IVehicleService _vehicleService;

    // Estado do noclip
    private bool _isNoclipActive;

    public PlayerController(ILogger logger, IPlayerService playerService, IVehicleService vehicleService)
    {
      _logger = logger;
      _playerService = playerService;
      _vehicleService = vehicleService;
      _logger.Info("PlayerController initialized!");

      API.RegisterCommand("heal", new Action<int, List<object>, string>((source, args, raw) =>
      {
        HealCommand();
      }), false);

      API.RegisterCommand("tp", new Action<int, List<object>, string>((source, args, raw) =>
      {
        string[] strArgs = args.Select(a => a.ToString()).ToArray();
        TeleportCommand(strArgs);
      }), false);

      API.RegisterCommand("name", new Action<int, List<object>, string>((source, args, raw) =>
      {
        NameCommand();
      }), false);

      API.RegisterCommand("invehicle", new Action<int, List<object>, string>((source, args, raw) =>
      {
        InVehicleCommand();
      }), false);

      API.RegisterCommand("delcar", new Action<int, List<object>, string>((source, args, raw) =>
      {
        DeleteVehicleCommand();
      }), false);

      API.RegisterCommand("nc", new Action<int, List<object>, string>((source, args, raw) =>
      {
        ToggleNoclip();
      }), false);
    }

    private async void HealCommand()
    {
      await _playerService.HealPlayer();
    }

    private async void TeleportCommand(string[] args)
    {
      if (args.Length < 3)
      {
        _logger.Info("Uso: /tp <x> <y> <z>");
        return;
      }

      if (float.TryParse(args[0], out float x) && 
          float.TryParse(args[1], out float y) && 
          float.TryParse(args[2], out float z))
      {
        await _playerService.TeleportTo(x, y, z);
      }
      else
      {
        _logger.Error("Coordenadas inválidas!");
      }
    }

    private void NameCommand()
    {
      string playerName = _playerService.GetPlayerName();
      _logger.Info($"Nome do jogador: {playerName}");
    }

    private void InVehicleCommand()
    {
      bool isInVehicle = _playerService.IsPlayerInVehicle();
      
      if (isInVehicle)
      {
        _logger.Info("Jogador está em um veículo!");
      }
      else
      {
        _logger.Info("Jogador não está em um veículo!");
      }
    }

    private async void DeleteVehicleCommand()
    {
      if (_playerService.IsPlayerInVehicle())
      {
        await _vehicleService.DeleteVehicle();
      }
      else
      {
        _logger.Warning("Jogador não está em um veículo!");
      }
    }

    private void ToggleNoclip()
    {
      _isNoclipActive = !_isNoclipActive;

      if (_isNoclipActive)
      {
        _logger.Info("Noclip ativado!");
        EnableNoclip();
      }
      else
      {
        _logger.Info("Noclip desativado!");
        DisableNoclip();
      }
    }

    private void EnableNoclip()
    {
      // Exemplo simples: tornar o jogador intangível e sem colisão
      var playerPed = API.PlayerPedId();
      API.SetEntityCollision(playerPed, false, false);
      API.SetEntityVisible(playerPed, false, false);
      API.SetEntityInvincible(playerPed, true);
      API.SetPedCanRagdoll(playerPed, false);

      // Você pode implementar o movimento noclip no Tick (não mostrado aqui)
    }

    private void DisableNoclip()
    {
      var playerPed = API.PlayerPedId();
      API.SetEntityCollision(playerPed, true, true);
      API.SetEntityVisible(playerPed, true, false);
      API.SetEntityInvincible(playerPed, false);
      API.SetPedCanRagdoll(playerPed, true);
    }
  }
}
