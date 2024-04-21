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

public class VoteMap
{
    private IStringLocalizer? Localizer;
    private string MapchoseName = "";
    private string ChosenMap = "";
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

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteMap_DisableItOnJoinTheseGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteMap_DisableItOnJoinTheseGroups))
        {
            if (!Globals_VoteMap.VoteMap_Disable.ContainsKey(playerid))
            {
                Globals_VoteMap.VoteMap_Disable.Add(playerid, true);
                Globals_VoteMap.VoteMap_Disabled = true;
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(string.IsNullOrEmpty(Configs.GetConfigData().VoteMap_CommandsToVote) || @event == null)return HookResult.Continue;
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
        string[] CenterMenuCommands = Configs.GetConfigData().VoteMap_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteMapMenu = new ChatMenu(Localizer!["votemap.menu.name"]);            
            
            var AllPlayers = Helper.GetAllController();
            var AllPlayersCount = Helper.GetAllCount();
            if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMap_DisableItOnJoinTheseGroups) && Globals_VoteMap.VoteMap_Disabled)
            {
                if (!string.IsNullOrEmpty(Localizer!["votemap.player.is.disabled"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votemap.player.is.disabled"]);
                }
                return HookResult.Continue;
            }

            if(AllPlayersCount < Configs.GetConfigData().VoteMap_StartOnMinimumOfXPlayers)
            {
                if (!string.IsNullOrEmpty(Localizer!["votemap.minimum.needed"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer!["votemap.minimum.needed"],Configs.GetConfigData().VoteMap_StartOnMinimumOfXPlayers);
                }
                return HookResult.Continue;
            }
            if(AllPlayersCount >= Configs.GetConfigData().VoteMap_StartOnMinimumOfXPlayers)
            {
                string jsonFilePath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/config/VoteMap.json");
                string jsonData = File.ReadAllText(jsonFilePath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
                if (data == null) return HookResult.Continue;

                foreach (var key in data.Keys)
                {
                    string display = data[key].ContainsKey("Display") ? data[key]["Display"] : key;
                    VoteMapMenu.AddMenuOption(display, (Caller, option) => HandleMenuALL(Caller, option, key));
                }
            }
            
            VoteMapMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteMapMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteMap_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {            
            if (Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteMap.VoteMap_ShowMenuBOTH[CallerSteamID])
            {
                if (!Globals_VoteMap.VoteMap_CallerVotedTo.ContainsKey(Caller))
                {
                    Globals_VoteMap.VoteMap_CallerVotedTo[Caller] = new HashSet<string>();
                }

                if (!Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMap.VoteMap_targetPlayerNameBOTH)
                {
                    Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Add(Globals_VoteMap.VoteMap_targetPlayerNameBOTH);
                    Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH] = Globals_VoteMap.VoteMap_GetVoted.ContainsKey(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) ? Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH] + 1 : 1;
                    Globals_VoteMap.VoteMap_countingBoth = Globals_VoteMap.VoteMap_GetVoted.ContainsKey(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) ? Globals_VoteMap.VoteMap_countingBoth + 1 : 1;
                    
                }else if (Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMap.VoteMap_targetPlayerNameBOTH)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votemap.player.vote.same.yes"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votemap.player.vote.same.yes"],Globals_VoteMap.VoteMap_targetPlayerNameBOTH, Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH], Globals_VoteMap.VoteMap_requiredboth);
                    }
                    return HookResult.Continue;
                }

                if (Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH] >= Globals_VoteMap.VoteMap_requiredboth)
                {
                    

                    if(Configs.GetConfigData().Log_SendLogToText)
                    {
                        var replacerlog = Helper.ReplaceMessagesMap(Configs.GetConfigData().Log_MapFormat, Date, Time, MapchoseName);
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
                    var replacerlogd = Helper.ReplaceMessagesMap(Configs.GetConfigData().Log_DiscordMapFormat, Date, Time, MapchoseName);
                    if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                    {
                        if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMapFormat))
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
                            if (ChosenMap.StartsWith("ds:") )
                            {
                                string dsworkshop = ChosenMap.TrimStart().Substring("ds:".Length).Trim();
                                Server.ExecuteCommand($"ds_workshop_changelevel {dsworkshop}");
                            }else if (ChosenMap.StartsWith("host:"))
                            {
                                string hostworkshop = ChosenMap.TrimStart().Substring("host:".Length).Trim();
                                Server.ExecuteCommand($"host_workshop_map {hostworkshop}");
                            }else if (!(ChosenMap.StartsWith("ds:") || ChosenMap.StartsWith("host:")))
                            {
                                Server.ExecuteCommand($"changelevel {ChosenMap}");
                            }
                        }, TimerFlags.STOP_ON_MAPCHANGE);
                    });
                    
                    

                    if (!string.IsNullOrEmpty(Localizer!["votemap.announce.map.successfully.message"]))
                    {
                        Helper.AdvancedPrintToServer(Localizer["votemap.announce.map.successfully.message"], Globals_VoteMap.VoteMap_targetPlayerNameBOTH);
                    }
                    if (Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(Globals_VoteMap.VoteMap_targetPlayerNameBOTH))
                    {
                        var playerbothall = Helper.GetAllController();
                        playerbothall.ForEach(player => 
                        {
                            Globals_VoteMap.VoteMap_CallerVotedTo.Remove(player);
                            Globals_VoteMap.VoteMap_GetVoted[ChosenMap] = 0;
                            Globals_VoteMap.VoteMap_countingBoth = 0;
                            Globals_VoteMap.VoteMap_ShowMenuBOTH.Remove(player.SteamID);
                        });
                    }
                }
            }
            
        }
        string[] Refuse = Configs.GetConfigData().VoteMap_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if (Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteMap.VoteMap_ShowMenuBOTH[CallerSteamID])
            {
                if (!Globals_VoteMap.VoteMap_CallerVotedTo.ContainsKey(Caller))
                {
                    Globals_VoteMap.VoteMap_CallerVotedTo[Caller] = new HashSet<string>();
                }

                if (Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMap.VoteMap_targetPlayerNameBOTH)
                {
                    Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Remove(Globals_VoteMap.VoteMap_targetPlayerNameBOTH);
                    Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH] = Globals_VoteMap.VoteMap_GetVoted.ContainsKey(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) ? Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH] - 1 : 1;
                    Globals_VoteMap.VoteMap_countingBoth = Globals_VoteMap.VoteMap_GetVoted.ContainsKey(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) ? Globals_VoteMap.VoteMap_countingBoth - 1 : 1;
                    
                }else if (!Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(Globals_VoteMap.VoteMap_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMap.VoteMap_targetPlayerNameBOTH)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votemap.player.vote.same.no"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votemap.player.vote.same.no"],Globals_VoteMap.VoteMap_targetPlayerNameBOTH, Globals_VoteMap.VoteMap_GetVoted[Globals_VoteMap.VoteMap_targetPlayerNameBOTH], Globals_VoteMap.VoteMap_requiredboth);
                    }
                    return HookResult.Continue;
                }
            }
            
        }
        return HookResult.Continue;
    }

    private void HandleMenuALL(CCSPlayerController Caller, ChatMenuOption option, string MapChoosen)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string DisplayMap = option.Text;
        string mapChoosen = MapChoosen;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";
        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var allPlayers = Helper.GetAllCount();
        float percentage = Configs.GetConfigData().VoteMap_Percentage;
        int requiredall = (int)Math.Ceiling(allPlayers * (percentage / 100.0f));

        if (!Globals_VoteMap.VoteMap_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteMap.VoteMap_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(mapChoosen))
        {
            
            Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Add(mapChoosen);
            Globals_VoteMap.VoteMap_GetVoted[mapChoosen] = Globals_VoteMap.VoteMap_GetVoted.ContainsKey(mapChoosen) ? Globals_VoteMap.VoteMap_GetVoted[mapChoosen] + 1 : 1;
            
        }else if (Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(mapChoosen))
        {
            if (!string.IsNullOrEmpty(Localizer!["votemap.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votemap.player.vote.same.player"],DisplayMap, Globals_VoteMap.VoteMap_GetVoted[mapChoosen], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteMap_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMap.VoteMap_GetVoted[mapChoosen] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteMap.VoteMap_ShowMenuBOTH.Add(steamid, true);
                }
            }
            MapchoseName = DisplayMap;
            ChosenMap = mapChoosen;
            Globals_VoteMap.VoteMap_timerBOTH = Configs.GetConfigData().VoteMap_CenterMessageAnnouncementTimer;
            Globals_VoteMap.VoteMap_stopwatchBOTH.Start();
            Globals_VoteMap.VoteMap_targetPlayerNameBOTH = mapChoosen;
            Globals_VoteMap.VoteMap_countingBoth = Globals_VoteMap.VoteMap_GetVoted[mapChoosen];
            Globals_VoteMap.VoteMap_requiredboth = requiredall;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votemap.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votemap.chat.message"], CallerName, DisplayMap, Globals_VoteMap.VoteMap_GetVoted[mapChoosen], requiredall);
        }
        
        if (Configs.GetConfigData().VoteMap_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMap.VoteMap_GetVoted[mapChoosen] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votemap.announce.halfvotes.chat.message"]))
        {
            var playerall = Helper.GetAllController();
            playerall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votemap.announce.halfvotes.chat.message"]);
            });
        }
        
        if (Globals_VoteMap.VoteMap_GetVoted[mapChoosen] >= requiredall && !string.IsNullOrEmpty(Localizer!["votemap.announce.map.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votemap.announce.map.successfully.message"], DisplayMap);
        }
        

        if (Globals_VoteMap.VoteMap_GetVoted[mapChoosen] >= requiredall)
        {
            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessagesMap(Configs.GetConfigData().Log_MapFormat, Date, Time, DisplayMap);
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
            var replacerlogd = Helper.ReplaceMessagesMap(Configs.GetConfigData().Log_DiscordMapFormat, Date, Time, DisplayMap);
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2 || Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMapFormat))
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

                    if (mapChoosen.StartsWith("ds:") )
                    {
                        string dsworkshop = mapChoosen.TrimStart().Substring("ds:".Length).Trim();
                        Server.ExecuteCommand($"ds_workshop_changelevel {dsworkshop}");
                    }else if (mapChoosen.StartsWith("host:"))
                    {
                        string hostworkshop = mapChoosen.TrimStart().Substring("host:".Length).Trim();
                        Server.ExecuteCommand($"host_workshop_map {hostworkshop}");
                    }else if (!(mapChoosen.StartsWith("ds:") || mapChoosen.StartsWith("host:")))
                    {
                        Server.ExecuteCommand($"changelevel {mapChoosen}");
                    }

                    
                }, TimerFlags.STOP_ON_MAPCHANGE);
            });

            
            if (Globals_VoteMap.VoteMap_CallerVotedTo[Caller].Contains(DisplayMap))
            {
                var playersall = Helper.GetAllController();
                playersall.ForEach(player => 
                {
                    Globals_VoteMap.VoteMap_CallerVotedTo.Remove(player);
                    Globals_VoteMap.VoteMap_GetVoted[DisplayMap] = 0;
                    Globals_VoteMap.VoteMap_countingBoth = 0;
                    Globals_VoteMap.VoteMap_ShowMenuBOTH.Remove(player.SteamID);
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

        if (Globals_VoteMap.VoteMap_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteMap.VoteMap_CallerVotedTo[player])
            {
                if (Globals_VoteMap.VoteMap_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteMap.VoteMap_GetVoted[votedPlayer]--;
                    Globals_VoteMap.VoteMap_countingBoth = Globals_VoteMap.VoteMap_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteMap.VoteMap_countingBoth - 1 : 1;
                }
            }
            Globals_VoteMap.VoteMap_CallerVotedTo.Remove(player);
        }

        if (Globals_VoteMap.VoteMap_Disable.ContainsKey(playerid))
        {
            Globals_VoteMap.VoteMap_Disable.Remove(playerid);
            foreach (var allplayers in Helper.GetAllController())
            {
                if(allplayers == null || !allplayers.IsValid)continue;
                var playerssteamid = allplayers.SteamID;
                if (!Globals_VoteMap.VoteMap_Disable.ContainsKey(playerssteamid))
                {
                    Globals_VoteMap.VoteMap_Disabled = false;
                }
            }
        }
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariablesVoteMap();
    }
}