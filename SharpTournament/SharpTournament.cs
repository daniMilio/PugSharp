﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using SharpTournament.Config;
using SharpTournament.Match.Contract;
using System.Text.Json;

namespace SharpTournament;

public class SharpTournament : BasePlugin, IMatchCallback
{
    private readonly ConfigProvider _ConfigProvider = new();
    private Match.Match? _Match;

    public override string ModuleName => "SharpTournament Plugin";

    public override string ModuleVersion => "0.0.1";

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Loading SharpTournament!");
    }


    private void ExecuteServerCommand(string command, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Server.ExecuteCommand($"{command} {value}");

        }
    }

    public void InitializeMatch(MatchConfig matchConfig)
    {
        Server.ExecuteCommand("sv_disable_teamselect_menu true");
        Server.ExecuteCommand("sv_human_autojoin_team 2");
        Server.ExecuteCommand("mp_warmuptime 6000");

        Server.ExecuteCommand("mp_overtime_enable true");
        Server.ExecuteCommand("mp_overtime_maxrounds 6");
        Server.ExecuteCommand("mp_maxrounds 24");
        Server.ExecuteCommand("mp_tournament 1");

        Server.ExecuteCommand("mp_team_timeout_time 30");
        Server.ExecuteCommand("mp_team_timeout_max 3");


        ExecuteServerCommand($"mp_endmatch_votenextmap", "false");

        ExecuteServerCommand($"mp_teamname_1", matchConfig.Team1.Name);
        ExecuteServerCommand($"mp_teamflag_1", matchConfig.Team1.Flag);
        ExecuteServerCommand($"mp_teamname_2", matchConfig.Team2.Name);
        ExecuteServerCommand($"mp_teamflag_2", matchConfig.Team2.Flag);

        _Match = new Match.Match(this, matchConfig);

        var players = GetAllPlayers();
        foreach (var player in players.Where(x => x.UserId.HasValue && x.UserId >= 0))
        {
            if (player.UserId != null && !_Match.TryAddPlayer(player))
            {
                KickPlayer(player.UserId.Value);
            }
        }
    }

    #region Commands

    [ConsoleCommand("st_loadconfig", "Load a match config")]
    public void OnCommandLoadConfig(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("Start loading match config!");
        if (command.ArgCount != 3)
        {
            Console.WriteLine("Url is required as Argument!");
            player?.PrintToCenter("Url is required as Argument!");

            return;

        }

        var url = command.ArgByIndex(1);
        var authToken = command.ArgByIndex(2);
        var result = _ConfigProvider.TryLoadConfigAsync(url, authToken).Result;
        if (result.Successful)
        {
            InitializeMatch(result.Config!);
        }
    }

    [ConsoleCommand("st_dumpmatch", "Load a match config")]
    public void OnCommandDumpMatch(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("################ dump match ################");
        Console.WriteLine(JsonSerializer.Serialize(_Match));
        Console.WriteLine("################ dump match ################");
    }

    [ConsoleCommand("ready", "Mark player as ready")]
    public void OnCommandReady(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            Console.WriteLine("Command Start has been called by the server. Player is required to be marked as ready");
            return;
        }

        _Match?.TogglePlayerIsReady(new Player(player));


        Console.WriteLine("Command ready called.");
    }


    [ConsoleCommand("banmap", "Set map to ban")]
    public void OnCommandBanMap(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("Command banmap called.");

        if (player == null)
        {
            Console.WriteLine("Command banmap has been called by the server. Player is required to ban a map");
            return;
        }

        if (command.ArgCount != 2)
        {
            player.PrintToChat("banmap requires exact one argument!");
        }

        var mapNumber = command.ArgByIndex(1);

        _Match?.BanMap(new Player(player), mapNumber);
    }

    [ConsoleCommand("voteteam", "Vote a teamsite for startup")]
    public void OnCommandVoteTeam(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("Command voteteam called.");

        if (player == null)
        {
            Console.WriteLine("Command voteteam has been called by the server. Player is required to vote a team");
            return;
        }

        if (command.ArgCount != 2)
        {
            player.PrintToChat("voteteam requires exact one argument!");
        }

        var team = command.ArgByIndex(1);

        _Match?.VoteTeam(new Player(player), team);
    }

    [ConsoleCommand("st_start", "Starts a match")]
    public void OnCommandStart(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            Console.WriteLine("Command Start has been called by the server.");
            return;
        }

        Console.WriteLine("Start Command called.");
    }

    [ConsoleCommand("st_pause", "Pauses the current match")]
    public void OnCommandPause(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            Console.WriteLine("Command Pause has been called by the server.");
            return;
        }

        Console.WriteLine("Pause Command called.");
    }

    #endregion

    [GameEventHandler]
    public HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var userId = @event.Userid;

        if (userId != null && (PlayerState(userId) == PlayerConnectedState.PlayerConnected || PlayerState(userId) == PlayerConnectedState.PlayerReconnecting))
        {
            // // Userid will give you a reference to a CCSPlayerController class
            Console.WriteLine($"Player {userId.PlayerName} has connected full!");

            if (_Match == null)
            {
                Console.WriteLine($"Player {userId.PlayerName} kicked because no match has been loaded!");
                KickPlayer(userId.UserId);
            }
            else
            {
                userId.PrintToChat($"Hello {userId.PlayerName}, welcome to match {_Match.Config.MatchId}");
                if (!_Match.TryAddPlayer(new Player(userId)) && userId.UserId != null)
                {
                    KickPlayer(userId.UserId.Value);
                }
            }
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var userId = @event.Userid;

        // // Userid will give you a reference to a CCSPlayerController class
        Console.WriteLine($"Player {userId.PlayerName} has disconnected!");

        _Match?.SetPlayerDisconnected(new Player(userId));

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        if (_Match != null)
        {
            var configTeam = _Match.GetPlayerTeam(@event.Userid.SteamID);

            if ((int)configTeam != @event.Team)
            {
                Console.WriteLine($"Player {@event.Userid.PlayerName} tried to join {@event.Team} but is not allowed!");
                var player = new Player(@event.Userid);

                Server.NextFrame(() =>
                {
                    player.SwitchTeam(configTeam);
                    player.MatchStats?.ResetStats();
                });
                return HookResult.Continue;

            }
        }
        else
        {
            var players = GetAllPlayers();
            foreach (var player in players.Where(x => x.UserId != null))
            {
                KickPlayer(player.UserId!.Value);
            }
        }

        return HookResult.Continue;
    }



    private static void KickPlayer(int? userId)
    {
        if (userId == null)
        {
            return;
        }

        Server.ExecuteCommand($"kickid {userId} \"You are not part of the current match!\"");
    }

    #region Implementation of IMatchCallback

    public void SwitchMap(string selectedMap)
    {
        if (!Server.IsMapValid(selectedMap))
        {
            Console.WriteLine($"The selected map is not valid: \"{selectedMap}\"!");
            return;
        }

        Server.ExecuteCommand($"map {selectedMap}");
    }

    public IReadOnlyList<IPlayer> GetAllPlayers()
    {
        var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");

        return playerEntities.Where(x => PlayerState(x) == PlayerConnectedState.PlayerConnected).Select(p => new Player(p)).ToArray();
    }

    private static PlayerConnectedState PlayerState(CCSPlayerController player)
    {
        if (player == null || !player.IsValid)
        {
            return PlayerConnectedState.PlayerNeverConnected;
        }

        var statusRef = Schema.GetRef<UInt32>(player.Handle, "CBasePlayerController", "m_iConnected");

        return (PlayerConnectedState)statusRef;
    }

    public enum PlayerConnectionState
    {
        PlayerNeverConnected = 0xfffffff,
        PlayerConnected = 0x0,
        PlayerConnecting = 0x1,
        PlayerReconnecting = 0x2,
        PlayerDisconnecting = 0x3,
        PlayerDisconnected = 0x4,
        PlayerReserved = 0x5,
    }

    public IReadOnlyCollection<string> GetAvailableMaps()
    {
        return Server.GetMapList().ToList();
    }

    public void SendMessage(string message)
    {
        Server.PrintToChatAll(message);
    }

    public void EndWarmup()
    {
        Server.ExecuteCommand("mp_warmup_end");
    }

    public void PauseServer()
    {
        // TODO Oder lieber mp_pause_match/mp_unpause_match sodass erst am ende der Runde eine pause gestartet wird?
        Server.ExecuteCommand("pause");
    }

    public void UnpauseServer()
    {
        Server.ExecuteCommand("unpause");
    }
    #endregion


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _ConfigProvider.Dispose();
        }
    }

}