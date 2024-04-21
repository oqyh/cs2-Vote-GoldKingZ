using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Localization;
using Vote_GoldKingZ.Config;

namespace Vote_GoldKingZ;

[MinimumApiVersion(164)]
public class VoteGoldKingZ : BasePlugin
{
    public override string ModuleName => "Vote (Kick, Banned, Mute, Gag, Silent, Gamemode, Map, Vips)";
    public override string ModuleVersion => "1.0.9";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    
    private readonly VoteAdmin _VoteAdmin= new();
    private readonly VoteKick _VoteKick = new();
    private readonly VoteBanned _VoteBanned = new();
    private readonly VoteMute _VoteMute = new();
    private readonly VoteGag _VoteGag = new();
    private readonly VoteSilent _VoteSilent = new();
    private readonly VoteGameMode _VoteGameMode = new();
    private readonly VoteMap _VoteMap = new();
    private readonly VoteKickCenterAnnouncement _VoteKickCenterAnnouncement = new();
    private readonly VoteBannedCenterAnnouncement _VoteBannedCenterAnnouncement = new();
    private readonly VoteMuteCenterAnnouncement _VoteMuteCenterAnnouncement = new();
    private readonly VoteGagCenterAnnouncement _VoteGagCenterAnnouncement = new();
    private readonly VoteSilentCenterAnnouncement _VoteSilentCenterAnnouncement = new();
    private readonly VoteGameModeCenterAnnouncement _VoteGameModeCenterAnnouncement = new();
    private readonly VoteMapCenterAnnouncement _VoteMapCenterAnnouncement = new();
    internal static IStringLocalizer? Stringlocalizer;
    
    public override void Load(bool hotReload)
    {
        string ModulePath = ModuleDirectory;
        Configs.Shared.CookiesFolderPath = ModulePath;
        Configs.Load(ModulePath, Server.GameDirectory);

        Stringlocalizer = Localizer;
        _VoteAdmin.SetStringLocalizer(Localizer);
        _VoteKick.SetStringLocalizer(Localizer);
        _VoteKickCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteBanned.SetStringLocalizer(Localizer);
        _VoteBannedCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteMute.SetStringLocalizer(Localizer);
        _VoteMuteCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteGag.SetStringLocalizer(Localizer);
        _VoteGagCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteSilent.SetStringLocalizer(Localizer);
        _VoteSilentCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteGameMode.SetStringLocalizer(Localizer);
        _VoteGameModeCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteMap.SetStringLocalizer(Localizer);
        _VoteMapCenterAnnouncement.SetStringLocalizer(Localizer);

        AddCommandListener("say", OnPlayerSayPublic, HookMode.Pre);
        AddCommandListener("say_team", OnPlayerSayTeam, HookMode.Pre);
        RegisterListener<Listeners.OnClientPutInServer>(OnClientPutInServer);
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterEventHandler<EventPlayerChat>(OnEventPlayerChat, HookMode.Post);
        RegisterEventHandler<EventPlayerConnectFull>(OnEventPlayerConnectFull);
    }
    
    public void OnMapStart(string mapname)
    {
        if(Configs.GetConfigData().Log_AutoDeleteLogsMoreThanXdaysOld > 0)
        {
            string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Vote-GoldKingZ/logs");
            Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.GetConfigData().Log_AutoDeleteLogsMoreThanXdaysOld));
        }
    }
    public HookResult OnPlayerSayPublic(CCSPlayerController? Caller, CommandInfo info)
    {
        if(Configs.GetConfigData().VoteGag)
        {
            if (Caller == null || !Caller.IsValid)return HookResult.Continue;
            var CallerSteamID = Caller.SteamID;
            var message = info.GetArg(1);
            if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
            string trimmedMessageStart = message.TrimStart();
            string trimmedMessage = trimmedMessageStart.TrimEnd();
            if (Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(CallerSteamID))
            {
                if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_LetTheseAllowedForGagedPlayers))
                {
                    string VoteGagLetTheseAllowedForGagedPlayers = Configs.GetConfigData().VoteGag_LetTheseAllowedForGagedPlayers;
                    string[] VoteGagLetTheseAllowedForGagedPlayer = VoteGagLetTheseAllowedForGagedPlayers.Split(',');
                    if (VoteGagLetTheseAllowedForGagedPlayer.Any(command => trimmedMessageStart.StartsWith(command.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        return HookResult.Continue;
                    }
                }
                return HookResult.Handled;
            }
        }
        if(Configs.GetConfigData().VoteSilent)
        {
            if (Caller == null || !Caller.IsValid)return HookResult.Continue;
            var CallerSteamID = Caller.SteamID;
            var message = info.GetArg(1);
            if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
            string trimmedMessageStart = message.TrimStart();
            string trimmedMessage = trimmedMessageStart.TrimEnd();
            if (Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(CallerSteamID))
            {
                if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_LetTheseAllowedForSilentedPlayers))
                {
                    string VoteSilentLetTheseAllowedForSilentedPlayers = Configs.GetConfigData().VoteSilent_LetTheseAllowedForSilentedPlayers;
                    string[] VoteSilentLetTheseAllowedForSilentedPlayer = VoteSilentLetTheseAllowedForSilentedPlayers.Split(',');
                    if (VoteSilentLetTheseAllowedForSilentedPlayer.Any(command => trimmedMessageStart.StartsWith(command.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        return HookResult.Continue;
                    }
                }
                return HookResult.Handled;
            }
        }
        return HookResult.Continue;
    }
    public HookResult OnPlayerSayTeam(CCSPlayerController? Caller, CommandInfo info)
    {
        if(Configs.GetConfigData().VoteGag)
        {
            if (Caller == null || !Caller.IsValid)return HookResult.Continue;
            var CallerSteamID = Caller.SteamID;
            var message = info.GetArg(1);
            if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
            string trimmedMessageStart = message.TrimStart();
            string trimmedMessage = trimmedMessageStart.TrimEnd();
            if (Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(CallerSteamID))
            {
                if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_LetTheseAllowedForGagedPlayers))
                {
                    string VoteGagLetTheseAllowedForGagedPlayers = Configs.GetConfigData().VoteGag_LetTheseAllowedForGagedPlayers;
                    string[] VoteGagLetTheseAllowedForGagedPlayer = VoteGagLetTheseAllowedForGagedPlayers.Split(',');
                    if (VoteGagLetTheseAllowedForGagedPlayer.Any(command => trimmedMessageStart.StartsWith(command.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        return HookResult.Continue;
                    }
                }
                return HookResult.Handled;
            }
        }
        if(Configs.GetConfigData().VoteSilent)
        {
            if (Caller == null || !Caller.IsValid)return HookResult.Continue;
            var CallerSteamID = Caller.SteamID;
            var message = info.GetArg(1);
            if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
            string trimmedMessageStart = message.TrimStart();
            string trimmedMessage = trimmedMessageStart.TrimEnd();
            if (Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(CallerSteamID))
            {
                if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_LetTheseAllowedForSilentedPlayers))
                {
                    string VoteSilentLetTheseAllowedForSilentedPlayers = Configs.GetConfigData().VoteSilent_LetTheseAllowedForSilentedPlayers;
                    string[] VoteSilentLetTheseAllowedForSilentedPlayer = VoteSilentLetTheseAllowedForSilentedPlayers.Split(',');
                    if (VoteSilentLetTheseAllowedForSilentedPlayer.Any(command => trimmedMessageStart.StartsWith(command.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        return HookResult.Continue;
                    }
                }
                return HookResult.Handled;
            }
        }
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups))
        {
            _VoteAdmin.OnEventPlayerChat(@event, info);
        }

        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            _VoteKick.OnEventPlayerChat(@event, info);
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            _VoteBanned.OnEventPlayerChat(@event, info);
        }

        if(Configs.GetConfigData().VoteMute)
        {
            _VoteMute.OnEventPlayerChat(@event, info);
        }

        if(Configs.GetConfigData().VoteGag)
        {
            _VoteGag.OnEventPlayerChat(@event, info);
        }

        if(Configs.GetConfigData().VoteSilent)
        {
            _VoteSilent.OnEventPlayerChat(@event, info);
        }
        if(Configs.GetConfigData().VoteGameMode)
        {
            _VoteGameMode.OnEventPlayerChat(@event, info);
        }
        if(Configs.GetConfigData().VoteMap)
        {
            _VoteMap.OnEventPlayerChat(@event, info);
        }
        return HookResult.Continue;
    }
    
    public void OnTick()
    {
        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            _VoteKickCenterAnnouncement.OnTick();
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            _VoteBannedCenterAnnouncement.OnTick();
        }
        
        if(Configs.GetConfigData().VoteMute)
        {
            _VoteMuteCenterAnnouncement.OnTick();
        }
        
        if(Configs.GetConfigData().VoteGag)
        {
            _VoteGagCenterAnnouncement.OnTick();
        }

        if(Configs.GetConfigData().VoteSilent)
        {
            _VoteSilentCenterAnnouncement.OnTick();
        }
        if(Configs.GetConfigData().VoteGameMode)
        {
            _VoteGameModeCenterAnnouncement.OnTick();
        }
        if(Configs.GetConfigData().VoteMap)
        {
            _VoteMapCenterAnnouncement.OnTick();
        }
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        
        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups))
        {
            _VoteAdmin.OnEventPlayerConnectFull(@event, info);
        }

        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            _VoteKick.OnEventPlayerConnectFull(@event, info);
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            _VoteBanned.OnEventPlayerConnectFull(@event, info);
        }

        if(Configs.GetConfigData().VoteMute)
        {
            _VoteMute.OnEventPlayerConnectFull(@event, info);
        }

        if(Configs.GetConfigData().VoteGag)
        {
            _VoteGag.OnEventPlayerConnectFull(@event, info);
        }
        if(Configs.GetConfigData().VoteSilent)
        {
            _VoteSilent.OnEventPlayerConnectFull(@event, info);
        }
        if(Configs.GetConfigData().VoteGameMode)
        {
            _VoteGameMode.OnEventPlayerConnectFull(@event, info);
        }
        if(Configs.GetConfigData().VoteMap)
        {
            _VoteMap.OnEventPlayerConnectFull(@event, info);
        }
        return HookResult.Continue;
    }
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups))
        {
            _VoteAdmin.OnPlayerDisconnect(@event, info);
        }

        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            _VoteKick.OnPlayerDisconnect(@event, info);
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            _VoteBanned.OnPlayerDisconnect(@event, info);
        }

        if(Configs.GetConfigData().VoteMute)
        {
            _VoteMute.OnPlayerDisconnect(@event, info);
        }
        
        if(Configs.GetConfigData().VoteGag)
        {
            _VoteGag.OnPlayerDisconnect(@event, info);
        }

        if(Configs.GetConfigData().VoteSilent)
        {
            _VoteSilent.OnPlayerDisconnect(@event, info);
        }

        if(Configs.GetConfigData().VoteGameMode)
        {
            _VoteGameMode.OnPlayerDisconnect(@event, info);
        }
        if(Configs.GetConfigData().VoteMap)
        {
            _VoteMap.OnPlayerDisconnect(@event, info);
        }
        return HookResult.Continue;
    }
    public void OnClientPutInServer(int playerSlot)
    {
        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            _VoteKick.OnClientPutInServer(playerSlot);
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            _VoteBanned.OnClientPutInServer(playerSlot);
        }
    }

    public void OnMapEnd()
    {
        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups))
        {
            _VoteAdmin.OnMapEnd();
        }

        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            _VoteKick.OnMapEnd();
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            _VoteBanned.OnMapEnd();
        }

        if(Configs.GetConfigData().VoteMute)
        {
            _VoteMute.OnMapEnd();
        }

        if(Configs.GetConfigData().VoteGag)
        {
            _VoteGag.OnMapEnd();
        }
        if(Configs.GetConfigData().VoteSilent)
        {
            _VoteSilent.OnMapEnd();
        }
        if(Configs.GetConfigData().VoteGameMode)
        {
            _VoteGameMode.OnMapEnd();
        }
        if(Configs.GetConfigData().VoteMap)
        {
            _VoteMap.OnMapEnd();
        }
    }

    public override void Unload(bool hotReload)
    {
        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups))
        {
            Helper.ClearVariablesVoteAdmin();
        }

        if(Configs.GetConfigData().VoteKick_Mode != 0)
        {
            Helper.ClearVariablesVoteKick();
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode != 0)
        {
            Helper.ClearVariablesVoteBan();
        }

        if(Configs.GetConfigData().VoteMute)
        {
            Helper.ClearVariablesVoteMute();
        }

        if(Configs.GetConfigData().VoteGag)
        {
            Helper.ClearVariablesVoteGag();
        }
        if(Configs.GetConfigData().VoteSilent)
        {
            Helper.ClearVariablesVoteSilent();
        }
        if(Configs.GetConfigData().VoteGameMode)
        {
            Helper.ClearVariablesVoteGameMode();
        }
        if(Configs.GetConfigData().VoteMap)
        {
            Helper.ClearVariablesVoteMap();
        }
    }
}