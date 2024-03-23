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
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    
    private readonly VoteKick _VoteKick = new();
    private readonly VoteBanned _VoteBanned = new();
    private readonly VoteKickCenterAnnouncement _VoteKickCenterAnnouncement = new();
    private readonly VoteBannedCenterAnnouncement _VoteBannedCenterAnnouncement = new();
    internal static IStringLocalizer? Stringlocalizer;
    public bool missingdll = false;
    public bool missingwriteing = false;
    public bool missingreading = false;
    
    public override void Load(bool hotReload)
    {
        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| CHECKING FILES... ||||||||||||||||||||||||||||||||||||||||||||||||");
        Console.WriteLine("[Vote-GoldKingZ] Checking all files Missing...");
        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| CHECKING FILES... ||||||||||||||||||||||||||||||||||||||||||||||||");
        string dllFilePath = Path.Combine(ModuleDirectory, "Newtonsoft.Json.dll");
        string folder = Path.Combine(ModuleDirectory, "../../plugins/Vote-GoldKingZ");
        Server.NextFrame(() =>
        {
            if(!File.Exists(dllFilePath))
            {
                AddTimer(6.30f, () =>
                {
                    missingdll = true;
                }, TimerFlags.STOP_ON_MAPCHANGE);
            }
        });
        Server.NextFrame(() =>
        {
            AddTimer(8.30f, () =>
            {
                if(!Helper.IsDirectoryReadable(ModuleDirectory))
                {
                    missingreading = true;
                }
            }, TimerFlags.STOP_ON_MAPCHANGE);
        });
        Server.NextFrame(() =>
        {
            AddTimer(9.30f, () =>
            {
                if(!Helper.IsDirectoryWritable(ModuleDirectory))
                {
                    missingwriteing = true;
                }
            }, TimerFlags.STOP_ON_MAPCHANGE);
        });
        
        Server.NextFrame(() =>
        {
            AddTimer(10.30f, () =>
            {
                if(!missingdll && !missingwriteing && !missingreading)
                {
                    Stringlocalizer = Localizer;
                    _VoteKick.SetStringLocalizer(Localizer);
                    _VoteKickCenterAnnouncement.SetStringLocalizer(Localizer);
                    _VoteBanned.SetStringLocalizer(Localizer);
                    _VoteBannedCenterAnnouncement.SetStringLocalizer(Localizer);

                    RegisterListener<Listeners.OnClientPutInServer>(OnClientPutInServer);
                    RegisterListener<Listeners.OnTick>(OnTick);
                    RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
                    RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
                    RegisterEventHandler<EventPlayerChat>(OnEventPlayerChat, HookMode.Post);
                    RegisterEventHandler<EventPlayerConnectFull>(OnEventPlayerConnectFull);

                    string ModulePath = ModuleDirectory;
                    Configs.Shared.CookiesFolderPath = ModulePath;
                    Configs.Load(ModulePath);

                    Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| L O A D E D  ||||||||||||||||||||||||||||||||||||||||||||||||");
                    Console.WriteLine("[Vote-GoldKingZ] Plugin Vote-GoldKingZ.dll Loaded 100%");
                    Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| L O A D E D ||||||||||||||||||||||||||||||||||||||||||||||||");
                }else
                {
                    AddCommandListener("say", OnPlayerSayPublic, HookMode.Post);
                    Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| N O T - L O A D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    Console.WriteLine("[Vote-GoldKingZ]   ");
                    Console.WriteLine("[Vote-GoldKingZ] Plugin Failed To Load... Fix The Following...");
                    Console.WriteLine("[Vote-GoldKingZ]------------------------------------------------");
                    Console.WriteLine("[Vote-GoldKingZ]   ");
                    if(missingdll)
                    {
                        Console.WriteLine("[Vote-GoldKingZ] You Missing Newtonsoft.Json.dll Located In");
                        Console.WriteLine("[Vote-GoldKingZ] ../addons/counterstrikesharp/plugins/Vote-GoldKingZ/");
                        Console.WriteLine("[Vote-GoldKingZ] Please Add Newtonsoft.Json.dll Into Vote-GoldKingZ Folder To Load Plugin Correctly ");
                        Console.WriteLine("[Vote-GoldKingZ]   ");
                    }
                    if(missingwriteing)
                    {
                        Console.WriteLine("[Vote-GoldKingZ] Please Give This Folder Permision To ((Write))");
                        Console.WriteLine("[Vote-GoldKingZ] ../addons/counterstrikesharp/plugins/((Vote-GoldKingZ))");
                        Console.WriteLine("[Vote-GoldKingZ]  ");
                    }
                    if(missingreading)
                    {
                        Console.WriteLine("[Vote-GoldKingZ] Please Give This Folder Permision To ((Read))");
                        Console.WriteLine("[Vote-GoldKingZ] ../addons/counterstrikesharp/plugins/((Vote-GoldKingZ))");
                        Console.WriteLine("[Vote-GoldKingZ]   ");
                    }
                    Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| N O T - L O A D ||||||||||||||||||||||||||||||||||||||||||||||||");
                }
            }, TimerFlags.STOP_ON_MAPCHANGE);
        });
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;
        _VoteKick.OnEventPlayerChat(@event, info);
        _VoteBanned.OnEventPlayerChat(@event, info);
        return HookResult.Continue;
    }
    private HookResult OnPlayerSayPublic(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        if(missingdll || missingwriteing || missingreading)
        {
            Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| N O T - L O A D ||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("[Vote-GoldKingZ]   ");
            Console.WriteLine("[Vote-GoldKingZ] Plugin Failed To Load... Fix The Following...");
            Console.WriteLine("[Vote-GoldKingZ]------------------------------------------------");
            Console.WriteLine("[Vote-GoldKingZ]   ");
            if(missingdll)
            {
                Console.WriteLine("[Vote-GoldKingZ] You Missing Newtonsoft.Json.dll Located In");
                Console.WriteLine("[Vote-GoldKingZ] ../addons/counterstrikesharp/plugins/Vote-GoldKingZ/");
                Console.WriteLine("[Vote-GoldKingZ] Please Add Newtonsoft.Json.dll Into Vote-GoldKingZ Folder To Load Plugin Correctly ");
                Console.WriteLine("[Vote-GoldKingZ]   ");
            }
            if(missingwriteing)
            {
                Console.WriteLine("[Vote-GoldKingZ] Please Give This Folder Permision To ((Write))");
                Console.WriteLine("[Vote-GoldKingZ] ../addons/counterstrikesharp/plugins/((Vote-GoldKingZ))");
                Console.WriteLine("[Vote-GoldKingZ]  ");
            }
            if(missingreading)
            {
                Console.WriteLine("[Vote-GoldKingZ] Please Give This Folder Permision To ((Read))");
                Console.WriteLine("[Vote-GoldKingZ] ../addons/counterstrikesharp/plugins/((Vote-GoldKingZ))");
                Console.WriteLine("[Vote-GoldKingZ]   ");
            }
        }
        return HookResult.Continue;
    }
    public void OnTick()
    {
        _VoteKickCenterAnnouncement.OnTick();
        _VoteBannedCenterAnnouncement.OnTick();
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        _VoteKick.OnEventPlayerConnectFull(@event, info);
        _VoteBanned.OnEventPlayerConnectFull(@event, info);
        return HookResult.Continue;
    }
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        _VoteKick.OnPlayerDisconnect(@event, info);
        _VoteBanned.OnPlayerDisconnect(@event, info);
        return HookResult.Continue;
    }
    public void OnClientPutInServer(int playerSlot)
    {
        _VoteKick.OnClientPutInServer(playerSlot);
        _VoteBanned.OnClientPutInServer(playerSlot);
    }

    public void OnMapEnd()
    {
        _VoteKick.OnMapEnd();
        _VoteBanned.OnMapEnd();
    }

    public override void Unload(bool hotReload)
    {
        Helper.ClearVariablesVoteKick();
        Helper.ClearVariablesVoteBanned();
    }
}