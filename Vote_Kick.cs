using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using System.Diagnostics;
using System.Net;
using CounterStrikeSharp.API.Modules.Admin;

namespace Vote_Kick;

public class VoteKickConfig : BasePluginConfig
{
    [JsonPropertyName("ImmunityGroupsFromGettingVoted")] public string ImmunityGroupsFromGettingVoted { get; set; } = "#css/vip1,#css/vip2,#css/vip3";
    [JsonPropertyName("VoteTeamMateOnly")] public bool VoteTeamMateOnly { get; set; } = false;
    [JsonPropertyName("VotePercentage")] public float VotePercentage { get; set; } = 6;
    [JsonPropertyName("VoteMessageAnnounceOnHalfVotes")] public bool VoteMessageAnnounceOnHalfVotes { get; set; } = false;
    [JsonPropertyName("VoteTimer")] public float VoteTimer { get; set; } = 25;

    [JsonPropertyName("RestrictPlayersMethod")] public int RestrictPlayersMethod { get; set; } = 1;
    [JsonPropertyName("AfterKickGivePlayerXMinsFromJoining")] public int AfterKickGivePlayerXMinsFromJoining { get; set; } = 5;
    [JsonPropertyName("ResetKickedPlayersOnMapChange")] public bool ResetKickedPlayersOnMapChange { get; set; } = false;
}

public class VoteKick : BasePlugin, IPluginConfig<VoteKickConfig> 
{
    public override string ModuleName => "Vote Kick";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Vote Kick Players";
    internal static IStringLocalizer? Stringlocalizer;
    public VoteKickConfig Config { get; set; } = new VoteKickConfig();
    private Dictionary<string, int> playerVotes = new Dictionary<string, int>();
    private Dictionary<CCSPlayerController, HashSet<string>> callerVotes = new Dictionary<CCSPlayerController, HashSet<string>>();
    private static Dictionary<ulong, DateTime> restrictedUsers = new Dictionary<ulong, DateTime>();
    private static Dictionary<ulong, DateTime> restrictedUsersip = new Dictionary<ulong, DateTime>();
    private Stopwatch stopwatchCT = new Stopwatch();
    private Stopwatch stopwatchT = new Stopwatch();
    private Stopwatch stopwatchBOTH = new Stopwatch();

    private static object lockObject = new object();
    public string targetPlayerNameCT = "";
    public string targetPlayerNameT = "";
    public string targetPlayerNameBOTH = "";
    public bool ReachHalfVoteCT = false;
    public bool ReachHalfVoteT = false;
    public bool ReachHalfVoteBoth = false;
    public int countingCT;
    public int countingT;
    public int countingBoth;
    public int requiredct;
    public int requiredt;
    public int requiredboth;
    public float timerCT;
    public float timerT;
    public float timerBOTH;


    public void OnConfigParsed(VoteKickConfig config)
    {
        Config = config;
        timerCT = Config.VoteTimer;
        timerT = Config.VoteTimer;
        timerBOTH = Config.VoteTimer;
        Stringlocalizer = Localizer;
        if (Config.RestrictPlayersMethod < 0 || Config.RestrictPlayersMethod > 3)
        {
            config.RestrictPlayersMethod = 1;
            Console.WriteLine("|||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("RestrictPlayersMethod: is invalid, setting to default value (1) Please Choose 0 or 1 or 2 or 3.");
            Console.WriteLine("RestrictPlayersMethod (0) = Disable (Kick Only)");
            Console.WriteLine("RestrictPlayersMethod (1) = Kick And Restrict SteamID From Joining");
            Console.WriteLine("RestrictPlayersMethod (2) = Kick And Restrict IpAddress From Joining");
            Console.WriteLine("RestrictPlayersMethod (3) = Kick And Restrict SteamID And IpAddress From Joining");
            Console.WriteLine("|||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||");
        }
    }
    
    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnClientPutInServer>(OnClientPutInServer);
        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        AddCommandListener("say", OnPlayerSayPublic, HookMode.Post);
        AddCommandListener("say_team", OnPlayerSayTeam, HookMode.Post);
    }
    public void OnTick()
    {
        if(ReachHalfVoteCT == true)
        {
            if (countingCT >= requiredct)
            {
                timerCT = Config.VoteTimer;
                stopwatchCT.Stop();
                ReachHalfVoteCT = false;
            }
            var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
            if (timerCT > 0)
            {
                if (stopwatchCT.ElapsedMilliseconds >= 1000)
                {
                    timerCT--;
                    stopwatchCT.Restart();
                }
            }
            foreach (var player in playerEntities)
            {
                var playerteam = player.TeamNum;
                if(playerteam == (byte)CsTeam.CounterTerrorist)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font>", timerCT);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>Kick player: </font> <font color='lightblue'>{0} ?</font>", targetPlayerNameCT);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font class='fontSize-l' color='green'> [ {0} / {1} ] </font>", countingCT, requiredct);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>To Kick Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font>");
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>To Remove Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>");
                    var centerhtml = builder.ToString();
                    player?.PrintToCenterHtml(centerhtml);
                }
                
            }
            if (timerCT < 1)
            {
                timerCT = Config.VoteTimer;
                stopwatchCT.Stop();
                ReachHalfVoteCT = false;
            }
            
        }

        if(ReachHalfVoteT == true)
        {
            if (countingT >= requiredt)
            {
                timerT = Config.VoteTimer;
                stopwatchT.Stop();
                ReachHalfVoteT = false;
            }
            var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
            if (timerT > 0)
            {
                if (stopwatchT.ElapsedMilliseconds >= 1000)
                {
                    timerT--;
                    stopwatchT.Restart();
                }
            }
            foreach (var player in playerEntities)
            {
                var playerteam = player.TeamNum;
                if(playerteam == (byte)CsTeam.Terrorist)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font>", timerT);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>Kick player: </font> <font color='lightblue'>{0} ?</font>", targetPlayerNameT);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font class='fontSize-l' color='green'> [ {0} / {1} ] </font>", countingT, requiredt);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>To Kick Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font>");
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>To Remove Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>");
                    var centerhtml = builder.ToString();
                    player?.PrintToCenterHtml(centerhtml);
                }
                
            }
            if (timerT < 1)
            {
                timerT = Config.VoteTimer;
                stopwatchT.Stop();
                ReachHalfVoteT = false;
            }
            
        }

        if(ReachHalfVoteBoth == true)
        {
            if (countingBoth >= requiredboth)
            {
                timerBOTH = Config.VoteTimer;
                stopwatchBOTH.Stop();
                ReachHalfVoteBoth = false;
            }
            var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
            if (timerBOTH > 0)
            {
                if (stopwatchBOTH.ElapsedMilliseconds >= 1000)
                {
                    timerBOTH--;
                    stopwatchBOTH.Restart();
                }
            }
            foreach (var player in playerEntities)
            {
                var playerteam = player.TeamNum;
                if(playerteam == (byte)CsTeam.Terrorist || playerteam == (byte)CsTeam.CounterTerrorist)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font>", timerBOTH);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>Kick player: </font> <font color='lightblue'>{0} ?</font>", targetPlayerNameBOTH);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font class='fontSize-l' color='green'> [ {0} / {1} ] </font>", countingBoth, requiredboth);
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>To Kick Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font>");
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='grey'>To Remove Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>");
                    var centerhtml = builder.ToString();
                    player?.PrintToCenterHtml(centerhtml);
                }
                
            }
            if (timerBOTH < 1)
            {
                timerBOTH = Config.VoteTimer;
                stopwatchBOTH.Stop();
                ReachHalfVoteBoth = false;
            }
            
        }
        
    }

    private void OnClientConnected(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return;
        var steamid = player.SteamID;
        string playerip = player.IpAddress!;
        ulong ipAddressAsKey = IpToUlong(playerip);
        bool canJoinSTEAMID = CanJoinid(steamid);
        bool canJoinSTEAMIP = CanJoinip(ipAddressAsKey);
        if(Config.RestrictPlayersMethod == 1)
        {
            if (!canJoinSTEAMID)
            {
                Server.ExecuteCommand($"kick {player.PlayerName}");
            }
        }
        if(Config.RestrictPlayersMethod == 2)
        {
            if (!canJoinSTEAMIP)
            {
                Server.ExecuteCommand($"kick {player.PlayerName}");
            }
        }
        if(Config.RestrictPlayersMethod == 3)
        {
            if (!canJoinSTEAMID || !canJoinSTEAMIP)
            {
                Server.ExecuteCommand($"kick {player.PlayerName}");
            }
        }
    }
    private void OnClientPutInServer(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return;
        var steamid = player.SteamID;
        string playerip = player.IpAddress!;
        ulong ipAddressAsKey = IpToUlong(playerip);
        bool canJoinSTEAMID = CanJoinid(steamid);
        bool canJoinSTEAMIP = CanJoinip(ipAddressAsKey);
        if(Config.RestrictPlayersMethod == 1)
        {
            if (!canJoinSTEAMID)
            {
                Server.ExecuteCommand($"kick {player.PlayerName}");
            }
        }
        if(Config.RestrictPlayersMethod == 2)
        {
            if (!canJoinSTEAMIP)
            {
                Server.ExecuteCommand($"kick {player.PlayerName}");
            }
        }
        if(Config.RestrictPlayersMethod == 3)
        {
            if (!canJoinSTEAMID || !canJoinSTEAMIP)
            {
                Server.ExecuteCommand($"kick {player.PlayerName}");
            }
        }
    }
    
    [ConsoleCommand("css_votekick")]
    [CommandHelper(minArgs: 1, usage: "[target]")]
    public void OnVoteKickCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (caller == null || !caller.IsValid || caller.IsBot || caller.IsHLTV)return;

        var callerteam = caller.TeamNum;
        var targetResult = command.GetArgTargetResult(1);
        double xPercentage = Config.VotePercentage / 10.0;

        if(Config.VoteTeamMateOnly)
        {
            if (!callerVotes.ContainsKey(caller))
            {
                callerVotes[caller] = new HashSet<string>();
            }
            
            foreach (var player in targetResult.Players)
            {
                if (player != null)
                {
                    if(caller == player)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_YourSelf"]))
                        {
                            caller.PrintToChat(Localizer["Vote_YourSelf"]);
                        }
                        return;
                    }
                    if (!string.IsNullOrEmpty(Config.ImmunityGroupsFromGettingVoted))
                    {
                        string[] excludedGroups = Config.ImmunityGroupsFromGettingVoted.Split(',');
                        foreach (string group in excludedGroups)
                        {
                            if (AdminManager.PlayerInGroup(player, group))
                            {
                                if (!string.IsNullOrEmpty(Localizer["This_User_IS_VIP"]))
                                {
                                    caller.PrintToChat(Localizer["This_User_IS_VIP", player]);
                                }
                                return;
                            }
                        }
                    }
                    var playerteam = player.TeamNum;
                    if(callerteam != playerteam)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_TeamMateOnly"]))
                        {
                            caller.PrintToChat(Localizer["Vote_TeamMateOnly"]);
                        }
                        return;
                        
                    }

                    if(callerteam == (byte)CsTeam.CounterTerrorist && playerteam == (byte)CsTeam.CounterTerrorist)
                    {
                        string playerNameTEAMCT = player.PlayerName;
                        string playerip = player.IpAddress!;
                        ulong ipAddressAsKey = IpToUlong(playerip);
                        var playerid = player.SteamID;
                        if (!callerVotes[caller].Contains(playerNameTEAMCT))
                        {
                            callerVotes[caller].Add(playerNameTEAMCT);
                            playerVotes[playerNameTEAMCT] = playerVotes.ContainsKey(playerNameTEAMCT) ? playerVotes[playerNameTEAMCT] + 1 : 1;

                            var countct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsHLTV);

                            requiredct = (int)Math.Ceiling(countct * xPercentage);
                            countingCT = playerVotes[playerNameTEAMCT];

                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessage", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMCT], requiredct]);
                            }
                            
                            if(Config.VoteMessageAnnounceOnHalfVotes)
                            {
                                if (playerVotes[playerNameTEAMCT] >= requiredct/2)
                                {
                                    targetPlayerNameCT = playerNameTEAMCT;
                                    ReachHalfVoteCT = true;
                                    stopwatchCT.Start();
                                }
                                if (playerVotes[playerNameTEAMCT] >= requiredct)
                                {
                                    Server.ExecuteCommand($"kick {playerNameTEAMCT}");
                                    if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                                    {
                                        Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMCT], requiredct]);
                                    }
                                    if(Config.RestrictPlayersMethod == 1)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 2)
                                    {
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 3)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }
                                    //playerVotes.Remove(playerName);
                                }
                            }else
                            {
                                if (playerVotes[playerNameTEAMCT] >= requiredct)
                                {
                                    Server.ExecuteCommand($"kick {playerNameTEAMCT}");
                                    if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                                    {
                                        Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMCT], requiredct]);
                                    }
                                    if(Config.RestrictPlayersMethod == 1)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 2)
                                    {
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 3)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                            {
                                caller.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[playerNameTEAMCT], requiredct]);
                            }
                        }
                    }

                    if(callerteam == (byte)CsTeam.Terrorist && playerteam == (byte)CsTeam.Terrorist)
                    {
                        string playerNameTEAMT = player.PlayerName;
                        string playerip = player.IpAddress!;
                        ulong ipAddressAsKey = IpToUlong(playerip);
                        var playerid = player.SteamID;
                        if (!callerVotes[caller].Contains(playerNameTEAMT))
                        {
                            callerVotes[caller].Add(playerNameTEAMT);
                            playerVotes[playerNameTEAMT] = playerVotes.ContainsKey(playerNameTEAMT) ? playerVotes[playerNameTEAMT] + 1 : 1;

                            var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsHLTV);

                            requiredt = (int)Math.Ceiling(countt * xPercentage);
                            countingT = playerVotes[playerNameTEAMT];

                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessage", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMT], requiredt]);
                            }
                            if(Config.VoteMessageAnnounceOnHalfVotes)
                            {
                                if (playerVotes[playerNameTEAMT] >= requiredt/2)
                                {
                                    targetPlayerNameT = playerNameTEAMT;
                                    ReachHalfVoteT = true;
                                    stopwatchT.Start();
                                }
                                if (playerVotes[playerNameTEAMT] >= requiredt)
                                {
                                    Server.ExecuteCommand($"kick {playerNameTEAMT}");
                                    if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                                    {
                                        Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMT], requiredt]);
                                    }
                                    if(Config.RestrictPlayersMethod == 1)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 2)
                                    {
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 3)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }
                                    //playerVotes.Remove(playerName);
                                }
                            }else
                            {
                                if (playerVotes[playerNameTEAMT] >= requiredt)
                                {
                                    Server.ExecuteCommand($"kick {playerNameTEAMT}");
                                    if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                                    {
                                        Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMT], requiredt]);
                                    }
                                    if(Config.RestrictPlayersMethod == 1)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 2)
                                    {
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }else if(Config.RestrictPlayersMethod == 3)
                                    {
                                        AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                        AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                            {
                                caller.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[playerNameTEAMT], requiredt]);
                            }
                        }
                    }
                }
            }
        }else
        {
            if (!callerVotes.ContainsKey(caller))
            {
                callerVotes[caller] = new HashSet<string>();
            }
            
            foreach (var player in targetResult.Players)
            {
                if (player != null)
                {
                    if(caller == player)
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_YourSelf"]))
                        {
                            caller.PrintToChat(Localizer["Vote_YourSelf"]);
                        }
                        return;
                    }
                    if (!string.IsNullOrEmpty(Config.ImmunityGroupsFromGettingVoted))
                    {
                        string[] excludedGroups = Config.ImmunityGroupsFromGettingVoted.Split(',');
                        foreach (string group in excludedGroups)
                        {
                            if (AdminManager.PlayerInGroup(player, group))
                            {
                                if (!string.IsNullOrEmpty(Localizer["This_User_IS_VIP"]))
                                {
                                    caller.PrintToChat(Localizer["This_User_IS_VIP", player]);
                                }
                                return;
                            }
                        }
                    }
                    string playerNameTEAMBOTH = player.PlayerName;
                    string playerip = player.IpAddress!;
                    ulong ipAddressAsKey = IpToUlong(playerip);
                    var playerid = player.SteamID;
                    if (!callerVotes[caller].Contains(playerNameTEAMBOTH))
                    {
                        callerVotes[caller].Add(playerNameTEAMBOTH);
                        playerVotes[playerNameTEAMBOTH] = playerVotes.ContainsKey(playerNameTEAMBOTH) ? playerVotes[playerNameTEAMBOTH] + 1 : 1;
                        

                        var countct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsHLTV);
                        var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsHLTV);
                        int bothteamsCount = countct + countt;

                        requiredboth = (int)Math.Ceiling(bothteamsCount * xPercentage);
                        countingBoth = playerVotes[playerNameTEAMBOTH];
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMBOTH], requiredboth]);
                        }
                        if(Config.VoteMessageAnnounceOnHalfVotes)
                        {
                            if (playerVotes[playerNameTEAMBOTH] >= requiredboth/2)
                            {
                                targetPlayerNameBOTH = playerNameTEAMBOTH;
                                ReachHalfVoteBoth = true;
                                stopwatchBOTH.Start();
                            }
                            if (playerVotes[playerNameTEAMBOTH] >= requiredboth)
                            {
                                Server.ExecuteCommand($"kick {playerNameTEAMBOTH}");
                                if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                                {
                                    Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMBOTH], requiredboth]);
                                }
                                if(Config.RestrictPlayersMethod == 1)
                                {
                                    AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                }else if(Config.RestrictPlayersMethod == 2)
                                {
                                    AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                }else if(Config.RestrictPlayersMethod == 3)
                                {
                                    AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                }
                                //playerVotes.Remove(playerName);
                            }
                        }else
                        {
                            if (playerVotes[playerNameTEAMBOTH] >= requiredboth)
                            {
                                Server.ExecuteCommand($"kick {playerNameTEAMBOTH}");
                                if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                                {
                                    Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", caller.PlayerName, player.PlayerName, playerVotes[playerNameTEAMBOTH], requiredboth]);
                                }
                                if(Config.RestrictPlayersMethod == 1)
                                {
                                    AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                }else if(Config.RestrictPlayersMethod == 2)
                                {
                                    AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                }else if(Config.RestrictPlayersMethod == 3)
                                {
                                    AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                    AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            caller.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[playerNameTEAMBOTH], requiredboth]);
                        }
                    }
                }
            }
        }
        
    }
    private HookResult OnPlayerSayPublic(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        
        var message = info.GetArg(1);
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
        string trimmedMessage1 = message.TrimStart();
        string trimmedMessage = trimmedMessage1.TrimEnd();
        var callerteam = player.TeamNum;
        
        if(trimmedMessage.Equals("!yes", StringComparison.OrdinalIgnoreCase) || trimmedMessage.Equals("!y", StringComparison.OrdinalIgnoreCase))
        {
            var playerid = player.SteamID;
            string playerip = player.IpAddress!;
            ulong ipAddressAsKey = IpToUlong(playerip);
            if(ReachHalfVoteCT == true)
            {
                if (!callerVotes.ContainsKey(player))
                {
                    callerVotes[player] = new HashSet<string>();
                }
                if(callerteam == (byte)CsTeam.CounterTerrorist)
                {
                    if (!callerVotes[player].Contains(targetPlayerNameCT))
                    {
                        playerVotes[targetPlayerNameCT]++;
                        countingCT = playerVotes.ContainsKey(targetPlayerNameCT) ? countingCT + 1 : 1;
                        callerVotes[player].Add(targetPlayerNameCT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameCT, playerVotes[targetPlayerNameCT], requiredct]);
                        }

                        if (playerVotes[targetPlayerNameCT] >= requiredct)
                        {
                            Server.ExecuteCommand($"kick {targetPlayerNameCT}");
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", player.PlayerName, targetPlayerNameCT, playerVotes[targetPlayerNameCT], requiredct]);
                            }
                            if(Config.RestrictPlayersMethod == 1)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 2)
                            {
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 3)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameCT], requiredct]);
                        }
                    }
                }
                
            }
            if(ReachHalfVoteT == true)
            {
                if (!callerVotes.ContainsKey(player))
                {
                    callerVotes[player] = new HashSet<string>();
                }
                if(callerteam == (byte)CsTeam.Terrorist)
                {
                    if (!callerVotes[player].Contains(targetPlayerNameT))
                    {
                        playerVotes[targetPlayerNameT]++;
                        countingT = playerVotes.ContainsKey(targetPlayerNameT) ? countingT + 1 : 1;
                        callerVotes[player].Add(targetPlayerNameT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameT, playerVotes[targetPlayerNameT], requiredt]);
                        }

                        if (playerVotes[targetPlayerNameT] >= requiredt)
                        {
                            Server.ExecuteCommand($"kick {targetPlayerNameT}");
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", player.PlayerName, targetPlayerNameT, playerVotes[targetPlayerNameT], requiredt]);
                            }
                            if(Config.RestrictPlayersMethod == 1)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 2)
                            {
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 3)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameT], requiredt]);
                        }
                    }
                }
            }
            if(ReachHalfVoteBoth == true)
            {
                if (!callerVotes.ContainsKey(player))
                {
                    callerVotes[player] = new HashSet<string>();
                }
                if(callerteam == (byte)CsTeam.CounterTerrorist || callerteam == (byte)CsTeam.Terrorist)
                {
                    if (!callerVotes[player].Contains(targetPlayerNameBOTH))
                    {
                        playerVotes[targetPlayerNameBOTH]++;
                        countingBoth = playerVotes.ContainsKey(targetPlayerNameBOTH) ? countingBoth + 1 : 1;
                        callerVotes[player].Add(targetPlayerNameBOTH);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameBOTH, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }

                        if (playerVotes[targetPlayerNameBOTH] >= requiredboth)
                        {
                            Server.ExecuteCommand($"kick {targetPlayerNameT}");
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", player.PlayerName, targetPlayerNameBOTH, playerVotes[targetPlayerNameBOTH], requiredboth]);
                            }
                            if(Config.RestrictPlayersMethod == 1)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 2)
                            {
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 3)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }
                    }
                }
            }

        }
        if(trimmedMessage.Equals("!no", StringComparison.OrdinalIgnoreCase) || trimmedMessage.Equals("!n", StringComparison.OrdinalIgnoreCase))
        {
            if(ReachHalfVoteCT == true)
            {
                if(callerteam == (byte)CsTeam.CounterTerrorist)
                {
                    if (callerVotes[player].Contains(targetPlayerNameCT))
                    {
                        playerVotes[targetPlayerNameCT]--;
                        countingCT = playerVotes.ContainsKey(targetPlayerNameCT) ? countingCT - 1 : 1;
                        callerVotes[player].Remove(targetPlayerNameCT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameCT, playerVotes[targetPlayerNameCT], requiredct]);
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameCT], requiredct]);
                        }
                    }
                }
                
            }
            if(ReachHalfVoteT == true)
            {
                if(callerteam == (byte)CsTeam.Terrorist)
                {
                    if (callerVotes[player].Contains(targetPlayerNameT))
                    {
                        playerVotes[targetPlayerNameT]--;
                        countingT = playerVotes.ContainsKey(targetPlayerNameT) ? countingT - 1 : 1;
                        callerVotes[player].Remove(targetPlayerNameT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameT, playerVotes[targetPlayerNameT], requiredt]);
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameT], requiredt]);
                        }
                    }
                }
            }
            if(ReachHalfVoteBoth == true)
            {
                if(callerteam == (byte)CsTeam.Terrorist || callerteam == (byte)CsTeam.CounterTerrorist)
                {
                    if (callerVotes[player].Contains(targetPlayerNameBOTH))
                    {
                        playerVotes[targetPlayerNameBOTH]--;
                        countingBoth = playerVotes.ContainsKey(targetPlayerNameBOTH) ? countingBoth - 1 : 1;
                        callerVotes[player].Remove(targetPlayerNameBOTH);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameBOTH, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }
                    }
                }
            }

        }
        return HookResult.Continue;
    }
    private HookResult OnPlayerSayTeam(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return HookResult.Continue;
        
        var message = info.GetArg(1);
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;
        string trimmedMessage1 = message.TrimStart();
        string trimmedMessage = trimmedMessage1.TrimEnd();
        var callerteam = player.TeamNum;
        
        if(trimmedMessage.Equals("!yes", StringComparison.OrdinalIgnoreCase) || trimmedMessage.Equals("!y", StringComparison.OrdinalIgnoreCase))
        {
            var playerid = player.SteamID;
            string playerip = player.IpAddress!;
            ulong ipAddressAsKey = IpToUlong(playerip);
            if(ReachHalfVoteCT == true)
            {
                if (!callerVotes.ContainsKey(player))
                {
                    callerVotes[player] = new HashSet<string>();
                }
                if(callerteam == (byte)CsTeam.CounterTerrorist)
                {
                    if (!callerVotes[player].Contains(targetPlayerNameCT))
                    {
                        playerVotes[targetPlayerNameCT]++;
                        countingCT = playerVotes.ContainsKey(targetPlayerNameCT) ? countingCT + 1 : 1;
                        callerVotes[player].Add(targetPlayerNameCT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameCT, playerVotes[targetPlayerNameCT], requiredct]);
                        }

                        if (playerVotes[targetPlayerNameCT] >= requiredct)
                        {
                            Server.ExecuteCommand($"kick {targetPlayerNameCT}");
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", player.PlayerName, targetPlayerNameCT, playerVotes[targetPlayerNameCT], requiredct]);
                            }
                            if(Config.RestrictPlayersMethod == 1)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 2)
                            {
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 3)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameCT], requiredct]);
                        }
                    }
                }
                
            }
            if(ReachHalfVoteT == true)
            {
                if (!callerVotes.ContainsKey(player))
                {
                    callerVotes[player] = new HashSet<string>();
                }
                if(callerteam == (byte)CsTeam.Terrorist)
                {
                    if (!callerVotes[player].Contains(targetPlayerNameT))
                    {
                        playerVotes[targetPlayerNameT]++;
                        countingT = playerVotes.ContainsKey(targetPlayerNameT) ? countingT + 1 : 1;
                        callerVotes[player].Add(targetPlayerNameT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameT, playerVotes[targetPlayerNameT], requiredt]);
                        }

                        if (playerVotes[targetPlayerNameT] >= requiredt)
                        {
                            Server.ExecuteCommand($"kick {targetPlayerNameT}");
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", player.PlayerName, targetPlayerNameT, playerVotes[targetPlayerNameT], requiredt]);
                            }
                            if(Config.RestrictPlayersMethod == 1)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 2)
                            {
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 3)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameT], requiredt]);
                        }
                    }
                }
            }
            if(ReachHalfVoteBoth == true)
            {
                if (!callerVotes.ContainsKey(player))
                {
                    callerVotes[player] = new HashSet<string>();
                }
                if(callerteam == (byte)CsTeam.CounterTerrorist || callerteam == (byte)CsTeam.Terrorist)
                {
                    if (!callerVotes[player].Contains(targetPlayerNameBOTH))
                    {
                        playerVotes[targetPlayerNameBOTH]++;
                        countingBoth = playerVotes.ContainsKey(targetPlayerNameBOTH) ? countingBoth + 1 : 1;
                        callerVotes[player].Add(targetPlayerNameBOTH);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameBOTH, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }

                        if (playerVotes[targetPlayerNameBOTH] >= requiredboth)
                        {
                            Server.ExecuteCommand($"kick {targetPlayerNameT}");
                            if (!string.IsNullOrEmpty(Localizer["Vote_KickMessagePassed"]))
                            {
                                Server.PrintToChatAll(Localizer["Vote_KickMessagePassed", player.PlayerName, targetPlayerNameBOTH, playerVotes[targetPlayerNameBOTH], requiredboth]);
                            }
                            if(Config.RestrictPlayersMethod == 1)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 2)
                            {
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }else if(Config.RestrictPlayersMethod == 3)
                            {
                                AddToRestrictedUserId(playerid, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                                AddToRestrictedUserIp(ipAddressAsKey, TimeSpan.FromMinutes(Config.AfterKickGivePlayerXMinsFromJoining));
                            }
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }
                    }
                }
            }

        }
        if(trimmedMessage.Equals("!no", StringComparison.OrdinalIgnoreCase) || trimmedMessage.Equals("!n", StringComparison.OrdinalIgnoreCase))
        {
            if(ReachHalfVoteCT == true)
            {
                if(callerteam == (byte)CsTeam.CounterTerrorist)
                {
                    if (callerVotes[player].Contains(targetPlayerNameCT))
                    {
                        playerVotes[targetPlayerNameCT]--;
                        countingCT = playerVotes.ContainsKey(targetPlayerNameCT) ? countingCT - 1 : 1;
                        callerVotes[player].Remove(targetPlayerNameCT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameCT, playerVotes[targetPlayerNameCT], requiredct]);
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameCT], requiredct]);
                        }
                    }
                }
                
            }
            if(ReachHalfVoteT == true)
            {
                if(callerteam == (byte)CsTeam.Terrorist)
                {
                    if (callerVotes[player].Contains(targetPlayerNameT))
                    {
                        playerVotes[targetPlayerNameT]--;
                        countingT = playerVotes.ContainsKey(targetPlayerNameT) ? countingT - 1 : 1;
                        callerVotes[player].Remove(targetPlayerNameT);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameT, playerVotes[targetPlayerNameT], requiredt]);
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameT], requiredt]);
                        }
                    }
                }
            }
            if(ReachHalfVoteBoth == true)
            {
                if(callerteam == (byte)CsTeam.Terrorist || callerteam == (byte)CsTeam.CounterTerrorist)
                {
                    if (callerVotes[player].Contains(targetPlayerNameBOTH))
                    {
                        playerVotes[targetPlayerNameBOTH]--;
                        countingBoth = playerVotes.ContainsKey(targetPlayerNameBOTH) ? countingBoth - 1 : 1;
                        callerVotes[player].Remove(targetPlayerNameBOTH);

                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessage"]))
                        {
                            Server.PrintToChatAll(Localizer["Vote_KickMessage", player.PlayerName, targetPlayerNameBOTH, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }
                    }else
                    {
                        if (!string.IsNullOrEmpty(Localizer["Vote_KickMessageDuplicate"]))
                        {
                            player.PrintToChat(Localizer["Vote_KickMessageDuplicate", player.PlayerName, playerVotes[targetPlayerNameBOTH], requiredboth]);
                        }
                    }
                }
            }

        }
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController disconnectedPlayer = @event.Userid;
        if (callerVotes.ContainsKey(disconnectedPlayer))
        {
            foreach (var votedPlayer in callerVotes[disconnectedPlayer])
            {
                if (playerVotes.ContainsKey(votedPlayer))
                {
                    playerVotes[votedPlayer]--;
                    countingCT = playerVotes.ContainsKey(votedPlayer) ? countingCT - 1 : 1;
                    countingT = playerVotes.ContainsKey(votedPlayer) ? countingT - 1 : 1;
                    countingBoth = playerVotes.ContainsKey(votedPlayer) ? countingBoth - 1 : 1;
                }
            }
            callerVotes.Remove(disconnectedPlayer);
        }

        return HookResult.Continue;
    }

    static void AddToRestrictedUserId(ulong? userId, TimeSpan duration)
    {
        if (userId.HasValue)
        {
            lock (lockObject)
            {
                DateTime endTime = DateTime.Now.Add(duration);
                restrictedUsers[userId.Value] = endTime;
            }
        }
    }

    static bool CanJoinid(ulong? userId)
    {
        lock (lockObject)
        {
            if (userId.HasValue && restrictedUsers.TryGetValue(userId.Value, out DateTime endTime))
            {
                if (DateTime.Now < endTime)
                {
                    return false;
                }
                else
                {
                    restrictedUsers.Remove(userId.Value);
                }
            }
            return true;
        }
    }
    static void AddToRestrictedUserIp(ulong? userId, TimeSpan duration)
    {
        if (userId.HasValue)
        {
            lock (lockObject)
            {
                DateTime endTime = DateTime.Now.Add(duration);
                restrictedUsersip[userId.Value] = endTime;
            }
        }
    }

    static bool CanJoinip(ulong? userId)
    {
        lock (lockObject)
        {
            if (userId.HasValue && restrictedUsersip.TryGetValue(userId.Value, out DateTime endTime))
            {
                if (DateTime.Now < endTime)
                {
                    return false;
                }
                else
                {
                    restrictedUsersip.Remove(userId.Value);
                }
            }
            return true;
        }
    }

    private void OnMapEnd()
    {
        callerVotes.Clear();
        playerVotes.Clear();
        if(Config.ResetKickedPlayersOnMapChange)
        {
            restrictedUsers.Clear();
        }
    }

    public override void Unload(bool hotReload)
    {
        callerVotes.Clear();
        playerVotes.Clear();
        restrictedUsers.Clear();
    }
    private ulong IpToUlong(string? ipAddress)
    {
        if (IPAddress.TryParse(ipAddress, out IPAddress? ip))
        {
            if (ip != null)
            {
                byte[] bytes = ip.GetAddressBytes();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
                return BitConverter.ToUInt64(bytes, 0);
            }
        }
        return 0;
    }

}