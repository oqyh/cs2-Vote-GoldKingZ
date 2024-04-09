using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Menu;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using System.Diagnostics;
using System.Net;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Vote_GoldKingZ;

public class VoteGameMode
{
    private IStringLocalizer? Localizer;
    private string GamemodechoseName = "";
    private string Gamemodechosecfg = "";
    private string Execcfg = "";
    public void SetStringLocalizer(IStringLocalizer stringLocalizer)
    {
        Localizer = stringLocalizer;
    }
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteGameMode_DisableItOnJoinTheseGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteGameMode_DisableItOnJoinTheseGroups))
        {
            if (!Globals_VoteGameMode.VoteGameMode_Disable.ContainsKey(playerid))
            {
                Globals_VoteGameMode.VoteGameMode_Disable.Add(playerid, true);
                Globals_VoteGameMode.VoteGameMode_Disabled = true;
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(string.IsNullOrEmpty(Configs.GetConfigData().VoteGameMode_CommandsToVote) || @event == null)return HookResult.Continue;
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";

        var eventplayer = @event.Userid;
        var eventmessage = @event.Text;
        var Caller = Utilities.GetPlayerFromUserid(eventplayer);
        

        if (Caller == null || !Caller.IsValid)return HookResult.Continue;
        var CallerTeam = Caller.TeamNum;
        var CallerSteamID = Caller.SteamID;

        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();
        string[] CenterMenuCommands = Configs.GetConfigData().VoteGameMode_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteGameModeMenu = new ChatMenu(Localizer!["votegamemode.menu.name"]);            
            
            var AllPlayers = Helper.GetAllController();
            var AllPlayersCount = Helper.GetAllCount();
            if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGameMode_DisableItOnJoinTheseGroups) && Globals_VoteGameMode.VoteGameMode_Disabled)
            {
                if (!string.IsNullOrEmpty(Localizer!["votegamemode.player.is.disabled"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votegamemode.player.is.disabled"]);
                }
                return HookResult.Continue;
            }
            if(AllPlayersCount < Configs.GetConfigData().VoteGameMode_StartOnMinimumOfXPlayers)
            {
                if (!string.IsNullOrEmpty(Localizer!["votegamemode.minimum.needed"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer!["votegamemode.minimum.needed"],Configs.GetConfigData().VoteGameMode_StartOnMinimumOfXPlayers);
                }
                return HookResult.Continue;
            }
            if(AllPlayersCount >= Configs.GetConfigData().VoteGameMode_StartOnMinimumOfXPlayers)
            {
                string jsonFilePath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/config/GameMode.json");
                string jsonData = File.ReadAllText(jsonFilePath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
                if (data == null) return HookResult.Continue;

                foreach (var key in data.Keys)
                {
                    string gameModeChosen = data[key]["Config"];
                    VoteGameModeMenu.AddMenuOption(key, (Caller, option) => HandleMenuALL(Caller, option, gameModeChosen));
                }
            }
            
            VoteGameModeMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteGameModeMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteGameMode_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            
            if (Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH[CallerSteamID])
            {
                if (!Globals_VoteGameMode.VoteGameMode_CallerVotedTo.ContainsKey(Caller))
                {
                    Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller] = new HashSet<string>();
                }

                if (!Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH)
                {
                    Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Add(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH);
                    Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH] = Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) ? Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH] + 1 : 1;
                    Globals_VoteGameMode.VoteGameMode_countingBoth = Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) ? Globals_VoteGameMode.VoteGameMode_countingBoth + 1 : 1;
                    
                }else if (Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votegamemode.player.vote.same.yes"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votegamemode.player.vote.same.yes"],Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH, Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH], Globals_VoteGameMode.VoteGameMode_requiredboth);
                    }
                    return HookResult.Continue;
                }

                if (Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH] >= Globals_VoteGameMode.VoteGameMode_requiredboth)
                {
                    

                    if(Configs.GetConfigData().Log_SendLogToText)
                    {
                        var replacerlog = Helper.ReplaceMessagesMode(Configs.GetConfigData().Log_GameModeFormat, Date, Time, GamemodechoseName);
                        if(!Directory.Exists(Fpath))
                        {
                            Directory.CreateDirectory(Fpath);
                        }

                        if(!File.Exists(Tpath))
                        {
                            using (File.Create(Tpath)) { }
                        }

                        try
                        {
                            File.AppendAllLines(Tpath, new[]{replacerlog});
                        }catch
                        {

                        }
                    }
                    var replacerlogd = Helper.ReplaceMessagesMode(Configs.GetConfigData().Log_DiscordGameModeFormat, Date, Time, GamemodechoseName);
                    if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                    {
                        if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordGameModeFormat))
                        {
                            Task.Run(() =>
                            {
                                _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                            });
                        }
                    }
                    
                    VoteGoldKingZ pluginInstance = new VoteGoldKingZ();
                    Server.NextFrame(() =>
                    {
                        pluginInstance.AddTimer(10.30f, () =>
                        {
                            if (pluginInstance is IDisposable disposableInstance)
                            {
                                disposableInstance.Dispose();
                            }
                            Server.ExecuteCommand($"exec {Execcfg + Gamemodechosecfg}");
                            Server.PrintToConsole($"You Choosen {GamemodechoseName} || {Gamemodechosecfg} || exec {Execcfg + Gamemodechosecfg}");
                        }, TimerFlags.STOP_ON_MAPCHANGE);
                    });
                    
                    

                    if (!string.IsNullOrEmpty(Localizer!["votegamemode.announce.gamemode.successfully.message"]))
                    {
                        Helper.AdvancedPrintToServer(Localizer["votegamemode.announce.gamemode.successfully.message"], Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH);
                    }
                    if (Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH))
                    {
                        var playerbothall = Helper.GetAllController();
                        playerbothall.ForEach(player => 
                        {
                            Globals_VoteGameMode.VoteGameMode_CallerVotedTo.Remove(player);
                            Globals_VoteGameMode.VoteGameMode_GetVoted[GamemodechoseName] = 0;
                            Globals_VoteGameMode.VoteGameMode_countingBoth = 0;
                            Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.Remove(player.SteamID);
                        });
                    }
                }
            }
            
        }
        string[] Refuse = Configs.GetConfigData().VoteGameMode_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if (Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH[CallerSteamID])
            {
                if (!Globals_VoteGameMode.VoteGameMode_CallerVotedTo.ContainsKey(Caller))
                {
                    Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller] = new HashSet<string>();
                }

                if (Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH)
                {
                    Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Remove(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH);
                    Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH] = Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) ? Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH] - 1 : 1;
                    Globals_VoteGameMode.VoteGameMode_countingBoth = Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) ? Globals_VoteGameMode.VoteGameMode_countingBoth - 1 : 1;
                    
                }else if (!Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votegamemode.player.vote.same.no"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votegamemode.player.vote.same.no"],Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH, Globals_VoteGameMode.VoteGameMode_GetVoted[Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH], Globals_VoteGameMode.VoteGameMode_requiredboth);
                    }
                    return HookResult.Continue;
                }
            }
            
        }
        return HookResult.Continue;
    }

    private void HandleMenuALL(CCSPlayerController Caller, ChatMenuOption option, string GamemodeChoosen)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string ChoosenModeName = option.Text;
        string ChoosenModeCfg = GamemodeChoosen;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Exec = "Vote-GoldKingZ/";
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";
        DateTime personDate = DateTime.Now;

        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var allPlayers = Helper.GetAllCount();
        float percentage = Configs.GetConfigData().VoteGameMode_Percentage;
        int requiredall = (int)Math.Ceiling(allPlayers * (percentage / 100.0f));

        if (!Globals_VoteGameMode.VoteGameMode_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(ChoosenModeName))
        {
            
            Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Add(ChoosenModeName);
            Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] = Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(ChoosenModeName) ? Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] + 1 : 1;
            
        }else if (Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(ChoosenModeName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votegamemode.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votegamemode.player.vote.same.player"],ChoosenModeName, Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteGameMode_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.Add(steamid, true);
                }
            }
            GamemodechoseName = ChoosenModeName;
            Gamemodechosecfg = ChoosenModeCfg;
            Execcfg = Exec;
            Globals_VoteGameMode.VoteGameMode_timerBOTH = Configs.GetConfigData().VoteGameMode_CenterMessageAnnouncementTimer;
            Globals_VoteGameMode.VoteGameMode_stopwatchBOTH.Start();
            Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH = ChoosenModeName;
            Globals_VoteGameMode.VoteGameMode_countingBoth = Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName];
            Globals_VoteGameMode.VoteGameMode_requiredboth = requiredall;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votegamemode.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votegamemode.chat.message"], CallerName, ChoosenModeName, Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteGameMode_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votegamemode.announce.halfvotes.chat.message"]))
        {
            var playerall = Helper.GetAllController();
            playerall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votegamemode.announce.halfvotes.chat.message"]);
            });
        }
        
        if (Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votegamemode.announce.gamemode.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votegamemode.announce.gamemode.successfully.message"], ChoosenModeName);
        }
        

        if (Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] >= requiredall)
        {
            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessagesMode(Configs.GetConfigData().Log_GameModeFormat, Date, Time, ChoosenModeName);
                if(!Directory.Exists(Fpath))
                {
                    Directory.CreateDirectory(Fpath);
                }

                if(!File.Exists(Tpath))
                {
                    using (File.Create(Tpath)) { }
                }

                try
                {
                    File.AppendAllLines(Tpath, new[]{replacerlog});
                }catch
                {

                }
            }
            var replacerlogd = Helper.ReplaceMessagesMode(Configs.GetConfigData().Log_DiscordGameModeFormat, Date, Time, ChoosenModeName);
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordGameModeFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }

            VoteGoldKingZ pluginInstance = new VoteGoldKingZ();
            Server.NextFrame(() =>
            {
                pluginInstance.AddTimer(10.30f, () =>
                {
                    if (pluginInstance is IDisposable disposableInstance)
                    {
                        disposableInstance.Dispose();
                    }
                    Server.ExecuteCommand($"exec {Exec + ChoosenModeCfg}");
                    Server.PrintToConsole($"You Choosen {ChoosenModeName} || {ChoosenModeCfg} || exec {Exec + ChoosenModeCfg}");
                }, TimerFlags.STOP_ON_MAPCHANGE);
            });

            
            if (Globals_VoteGameMode.VoteGameMode_CallerVotedTo[Caller].Contains(ChoosenModeName))
            {
                var playersall = Helper.GetAllController();
                playersall.ForEach(player => 
                {
                    Globals_VoteGameMode.VoteGameMode_CallerVotedTo.Remove(player);
                    Globals_VoteGameMode.VoteGameMode_GetVoted[ChoosenModeName] = 0;
                    Globals_VoteGameMode.VoteGameMode_countingBoth = 0;
                    Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.Remove(player.SteamID);
                });
            }
        }
        MenuManager.CloseActiveMenu(Caller);
    }
    

    private void SelectExit(CCSPlayerController Caller, ChatMenuOption option)
    {
        MenuManager.CloseActiveMenu(Caller);
    }

    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        var playerid = player.SteamID;
        
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;

        if (Globals_VoteGameMode.VoteGameMode_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteGameMode.VoteGameMode_CallerVotedTo[player])
            {
                if (Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteGameMode.VoteGameMode_GetVoted[votedPlayer]--;
                    Globals_VoteGameMode.VoteGameMode_countingBoth = Globals_VoteGameMode.VoteGameMode_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteGameMode.VoteGameMode_countingBoth - 1 : 1;
                }
            }
            Globals_VoteGameMode.VoteGameMode_CallerVotedTo.Remove(player);
        }

        if (Globals_VoteGameMode.VoteGameMode_Disable.ContainsKey(playerid))
        {
            Globals_VoteGameMode.VoteGameMode_Disable.Remove(playerid);
            foreach (var allplayers in Helper.GetAllController())
            {
                if(allplayers == null || !allplayers.IsValid)continue;
                var playerssteamid = allplayers.SteamID;
                if (!Globals_VoteGameMode.VoteGameMode_Disable.ContainsKey(playerssteamid))
                {
                    Globals_VoteGameMode.VoteGameMode_Disabled = false;
                }
            }
        }
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariablesVoteGameMode();
    }
}