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
    public override string ModuleName => "Vote (Kick , Mute , Banned, Vips)";
    public override string ModuleVersion => "1.0.6";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    
    private readonly VoteKick _VoteKick = new();
    private readonly VoteBanned _VoteBanned = new();
    private readonly VoteMute _VoteMute = new();
    private readonly VoteKickCenterAnnouncement _VoteKickCenterAnnouncement = new();
    private readonly VoteBannedCenterAnnouncement _VoteBannedCenterAnnouncement = new();
    private readonly VoteMuteCenterAnnouncement _VoteMuteCenterAnnouncement = new();
    internal static IStringLocalizer? Stringlocalizer;
    public bool missingdll = false;
    
    public override void Load(bool hotReload)
    {
        string ModulePath = ModuleDirectory;
        Configs.Shared.CookiesFolderPath = ModulePath;
        Configs.Load(ModulePath);

        Stringlocalizer = Localizer;
        _VoteKick.SetStringLocalizer(Localizer);
        _VoteKickCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteBanned.SetStringLocalizer(Localizer);
        _VoteBannedCenterAnnouncement.SetStringLocalizer(Localizer);
        _VoteMute.SetStringLocalizer(Localizer);
        _VoteMuteCenterAnnouncement.SetStringLocalizer(Localizer);

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

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

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
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

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
        return HookResult.Continue;
    }
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
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
    }

    public override void Unload(bool hotReload)
    {
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
    }
}