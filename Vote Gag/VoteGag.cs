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

namespace Vote_GoldKingZ;

public class VoteGag
{
    private IStringLocalizer? Localizer;
    private string filename = "Gag.json";
    public void SetStringLocalizer(IStringLocalizer stringLocalizer)
    {
        Localizer = stringLocalizer;
    }
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;
        var playername = player.PlayerName;
        string PlayerIp = player.IpAddress?.Split(':')[0] ?? "InvildIpAdress";
        string Reason = "Gag Evasion";

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_DisableItOnJoinTheseGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteGag_DisableItOnJoinTheseGroups))
        {
            if (!Globals_VoteGag.VoteGag_Disable.ContainsKey(playerid))
            {
                Globals_VoteGag.VoteGag_Disable.Add(playerid, true);
                Globals_VoteGag.VoteGag_Disabled = true;
            }
        }

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_ImmunityGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteGag_ImmunityGroups))
        {
            if (!Globals_VoteGag.VoteGag_Immunity.ContainsKey(playerid))
            {
                Globals_VoteGag.VoteGag_Immunity.Add(playerid, true);
            }
        }
        
        Json_VoteGag.PersonData personDataIP = Json_VoteGag.RetrievePersonDataByIp(PlayerIp, Configs.GetConfigData().VoteGag_TimeInMins, filename);
        Json_VoteGag.PersonData personDataID = Json_VoteGag.RetrievePersonDataById(playerid, Configs.GetConfigData().VoteGag_TimeInMins, filename);
        Json_VoteGag.PersonData personDataREASON = Json_VoteGag.RetrievePersonDataByReason(Reason, Configs.GetConfigData().VoteGag_TimeInMins, filename);
        DateTime personDate = DateTime.Now;
        
        if(Json_VoteGag.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteGag_TimeInMins, filename) || Json_VoteGag.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteGag_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteGag_EvasionPunishment && (personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason || personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason))
            {
                Json_VoteGag.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteGag_EvasionPunishmentTimeInMins, Configs.GetConfigData().VoteGag_EvasionPunishmentTimeInMins, "Gag Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Gag Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Gag Evasion");
                if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                {
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                    {
                        Task.Run(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                        });
                    }
                }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                {
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                    {
                        Task.Run(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, playerid.ToString(), playername);
                        });
                    }
                }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                {
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                    {
                        Task.Run(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, playerid.ToString(), playername);
                        });
                    }
                }
            }
            if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(playerid))
            {
                Globals_VoteGag.VoteGag_PlayerGaged.Add(playerid, true);
            }
            VoteGoldKingZ pluginInstance = new VoteGoldKingZ();
            Server.NextFrame(() =>
            {
                pluginInstance.AddTimer(6.30f, () =>
                {
                    if (pluginInstance is IDisposable disposableInstance)
                    {
                        disposableInstance.Dispose();
                    }
                    if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
                    {
                        Helper.AdvancedPrintToChat(player, Localizer!["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
                    }
                }, TimerFlags.STOP_ON_MAPCHANGE);
            });
            
                
        }

        return HookResult.Continue;
    }
    
    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_CommandsToVote) || @event == null)return HookResult.Continue;
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
        string[] CenterMenuCommands = Configs.GetConfigData().VoteGag_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteGagMenu = new ChatMenu(Localizer!["votegag.menu.name"]);            
            if(Configs.GetConfigData().VoteGag_TeamOnly)
            {
                if(CallerTeam == (byte)CsTeam.CounterTerrorist)
                {
                    var AllCTPlayers = Helper.GetCounterTerroristController();
                    var AllCTPlayersCount = Helper.GetCounterTerroristCount();
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_DisableItOnJoinTheseGroups) && Globals_VoteGag.VoteGag_Disabled)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.is.disabled"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.is.disabled"]);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount < Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.minimum.needed"],Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount >= Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllCTPlayers)
                        {
                            if(Caller == players || players == null || !players.IsValid)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteGagMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuCT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
                if(CallerTeam == (byte)CsTeam.Terrorist)
                {
                    var AllTPlayers = Helper.GetTerroristController();
                    var AllTPlayersCount = Helper.GetTerroristCount();
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_DisableItOnJoinTheseGroups) && Globals_VoteGag.VoteGag_Disabled)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.is.disabled"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.is.disabled"]);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount < Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.minimum.needed"],Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount >= Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllTPlayers)
                        {
                            if(Caller == players || players == null || !players.IsValid)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteGagMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
            }else
            {
                var AllPlayers = Helper.GetAllController();
                var AllPlayersCount = Helper.GetAllCount();
                if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_DisableItOnJoinTheseGroups) && Globals_VoteGag.VoteGag_Disabled)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votegag.player.is.disabled"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.is.disabled"]);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount < Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votegag.minimum.needed"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.minimum.needed"],Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount >= Configs.GetConfigData().VoteGag_StartOnMinimumOfXPlayers)
                {
                    foreach (var players in AllPlayers)
                    {
                        if(Caller == players || players == null || !players.IsValid)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteGagMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuALL(Caller, option, TargetPlayersUserID));
                    }
                }
            }
            VoteGagMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteGagMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteGag_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteGag_TeamOnly)
            {
                if (Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteGag.VoteGag_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameCT)
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Add(Globals_VoteGag.VoteGag_targetPlayerNameCT);
                        Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameCT) ? Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT] + 1 : 1;
                        Globals_VoteGag.VoteGag_countingCT = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameCT) ? Globals_VoteGag.VoteGag_countingCT + 1 : 1;
                        
                    }else if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.player.vote.same.yes"],Globals_VoteGag.VoteGag_targetPlayerNameCT, Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT], Globals_VoteGag.VoteGag_requiredct);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT] >= Globals_VoteGag.VoteGag_requiredct)
                    {
                        
                        Json_VoteGag.SaveToJsonFile(Globals_VoteGag.VoteGag_targetPlayerSTEAMCT, Globals_VoteGag.VoteGag_targetPlayerNameCT, Globals_VoteGag.VoteGag_targetPlayerIPCT!.ToString(), personDate, Configs.GetConfigData().VoteGag_TimeInMins, Configs.GetConfigData().VoteGag_TimeInMins, "Vote Gaged", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteGag.VoteGag_targetPlayerNameCT, Globals_VoteGag.VoteGag_targetPlayerSTEAMCT.ToString(), Globals_VoteGag.VoteGag_targetPlayerIPCT!.ToString(), "Vote Gaged");
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
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, Globals_VoteGag.VoteGag_targetPlayerNameCT, Globals_VoteGag.VoteGag_targetPlayerSTEAMCT.ToString(), Globals_VoteGag.VoteGag_targetPlayerIPCT!.ToString(), "Vote Gaged");
                        if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteGag.VoteGag_targetPlayerSTEAMCT.ToString(), Globals_VoteGag.VoteGag_targetPlayerNameCT);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteGag.VoteGag_targetPlayerSTEAMCT.ToString(), Globals_VoteGag.VoteGag_targetPlayerNameCT);
                                });
                            }
                        }
                        
                        var playerctall = Helper.GetCounterTerroristController();
                        playerctall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteGag.VoteGag_targetPlayerSTEAMCT)
                            {
                                if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(steamid))
                                {
                                    Globals_VoteGag.VoteGag_PlayerGaged.Add(steamid, true);
                                }
                                if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votegag.announce.gaged.successfully.message"]))
                        {
                            playerctall.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.gaged.successfully.message"], Globals_VoteGag.VoteGag_targetPlayerNameCT)
                            );
                        }

                        if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameCT))
                        {
                            playerctall.ForEach(player => 
                            {
                                Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
                                Globals_VoteGag.VoteGag_GetVoted[player.PlayerName] = 0;
                                Globals_VoteGag.VoteGag_countingCT = 0;
                                Globals_VoteGag.VoteGag_ShowMenuCT.Remove(player.SteamID);
                            });
                        }
                    }
                }
                if (Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteGag.VoteGag_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameT)
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Add(Globals_VoteGag.VoteGag_targetPlayerNameT);
                        Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameT) ? Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT] + 1 : 1;
                        Globals_VoteGag.VoteGag_countingT = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameT) ? Globals_VoteGag.VoteGag_countingT + 1 : 1;
                        
                    }else if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.player.vote.same.yes"],Globals_VoteGag.VoteGag_targetPlayerNameT, Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT], Globals_VoteGag.VoteGag_requiredt);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT] >= Globals_VoteGag.VoteGag_requiredt)
                    {
                        
                        Json_VoteGag.SaveToJsonFile(Globals_VoteGag.VoteGag_targetPlayerSTEAMT, Globals_VoteGag.VoteGag_targetPlayerNameT, Globals_VoteGag.VoteGag_targetPlayerIPT!.ToString(), personDate, Configs.GetConfigData().VoteGag_TimeInMins, Configs.GetConfigData().VoteGag_TimeInMins, "Vote Gaged", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteGag.VoteGag_targetPlayerNameT, Globals_VoteGag.VoteGag_targetPlayerSTEAMT.ToString(), Globals_VoteGag.VoteGag_targetPlayerIPT!.ToString(), "Vote Gaged");
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
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, Globals_VoteGag.VoteGag_targetPlayerNameT, Globals_VoteGag.VoteGag_targetPlayerSTEAMT.ToString(), Globals_VoteGag.VoteGag_targetPlayerIPT!.ToString(), "Vote Gaged");
                        if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteGag.VoteGag_targetPlayerSTEAMT.ToString(), Globals_VoteGag.VoteGag_targetPlayerNameT);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteGag.VoteGag_targetPlayerSTEAMT.ToString(), Globals_VoteGag.VoteGag_targetPlayerNameT);
                                });
                            }
                        }
                        
                        
                        var playertall = Helper.GetTerroristController();
                        playertall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteGag.VoteGag_targetPlayerSTEAMT)
                            {
                                if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(steamid))
                                {
                                    Globals_VoteGag.VoteGag_PlayerGaged.Add(steamid, true);
                                }
                                if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votegag.announce.gaged.successfully.message"]))
                        {
                            playertall.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.gaged.successfully.message"], Globals_VoteGag.VoteGag_targetPlayerNameT)
                            );
                        }
                        if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameT))
                        {
                            playertall.ForEach(player => 
                            {
                                Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
                                Globals_VoteGag.VoteGag_GetVoted[player.PlayerName] = 0;
                                Globals_VoteGag.VoteGag_countingT = 0;
                                Globals_VoteGag.VoteGag_ShowMenuT.Remove(player.SteamID);
                            });
                        }
                    }
                }
            }else
            {
                if (Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteGag.VoteGag_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameBOTH)
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Add(Globals_VoteGag.VoteGag_targetPlayerNameBOTH);
                        Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) ? Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH] + 1 : 1;
                        Globals_VoteGag.VoteGag_countingBoth = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) ? Globals_VoteGag.VoteGag_countingBoth + 1 : 1;
                        
                    }else if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.player.vote.same.yes"],Globals_VoteGag.VoteGag_targetPlayerNameBOTH, Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH], Globals_VoteGag.VoteGag_requiredboth);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH] >= Globals_VoteGag.VoteGag_requiredboth)
                    {
                        
                        Json_VoteGag.SaveToJsonFile(Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH, Globals_VoteGag.VoteGag_targetPlayerNameBOTH, Globals_VoteGag.VoteGag_targetPlayerIPBOTH!.ToString(), personDate, Configs.GetConfigData().VoteGag_TimeInMins, Configs.GetConfigData().VoteGag_TimeInMins, "Vote Gaged", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteGag.VoteGag_targetPlayerNameBOTH, Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH.ToString(), Globals_VoteGag.VoteGag_targetPlayerIPBOTH!.ToString(), "Vote Gaged");
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
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, Globals_VoteGag.VoteGag_targetPlayerNameBOTH, Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH.ToString(), Globals_VoteGag.VoteGag_targetPlayerIPBOTH!.ToString(), "Vote Gaged");
                        if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH.ToString(), Globals_VoteGag.VoteGag_targetPlayerNameBOTH);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH.ToString(), Globals_VoteGag.VoteGag_targetPlayerNameBOTH);
                                });
                            }
                        }
                        
                        
                        var playerbothall = Helper.GetAllController();
                        playerbothall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH)
                            {
                                if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(steamid))
                                {
                                    Globals_VoteGag.VoteGag_PlayerGaged.Add(steamid, true);
                                }
                                if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votegag.announce.gaged.successfully.message"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["votegag.announce.gaged.successfully.message"], Globals_VoteGag.VoteGag_targetPlayerNameBOTH);
                        }
                        if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameBOTH))
                        {
                            playerbothall.ForEach(player => 
                            {
                                Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
                                Globals_VoteGag.VoteGag_GetVoted[player.PlayerName] = 0;
                                Globals_VoteGag.VoteGag_countingBoth = 0;
                                Globals_VoteGag.VoteGag_ShowMenuBOTH.Remove(player.SteamID);
                            });
                        }
                    }
                }
            }
        }
        string[] Refuse = Configs.GetConfigData().VoteGag_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteGag_TeamOnly)
            {
                if (Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteGag.VoteGag_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameCT)
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Remove(Globals_VoteGag.VoteGag_targetPlayerNameCT);
                        Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameCT) ? Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT] - 1 : 1;
                        Globals_VoteGag.VoteGag_countingCT = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameCT) ? Globals_VoteGag.VoteGag_countingCT - 1 : 1;
                        
                    }else if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.player.vote.same.no"],Globals_VoteGag.VoteGag_targetPlayerNameCT, Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameCT], Globals_VoteGag.VoteGag_requiredct);
                        }
                        return HookResult.Continue;
                    }
                    
                }
                if (Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteGag.VoteGag_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameT)
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Remove(Globals_VoteGag.VoteGag_targetPlayerNameT);
                        Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameT) ? Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT] - 1 : 1;
                        Globals_VoteGag.VoteGag_countingT = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameT) ? Globals_VoteGag.VoteGag_countingT - 1 : 1;
                        
                    }else if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameT) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.player.vote.same.no"],Globals_VoteGag.VoteGag_targetPlayerNameT, Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameT], Globals_VoteGag.VoteGag_requiredt);
                        }
                        return HookResult.Continue;
                    }
                }
            }else
            {
                if (Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteGag.VoteGag_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameBOTH)
                    {
                        Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Remove(Globals_VoteGag.VoteGag_targetPlayerNameBOTH);
                        Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) ? Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH] - 1 : 1;
                        Globals_VoteGag.VoteGag_countingBoth = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) ? Globals_VoteGag.VoteGag_countingBoth - 1 : 1;
                        
                    }else if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(Globals_VoteGag.VoteGag_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteGag.VoteGag_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votegag.player.vote.same.no"],Globals_VoteGag.VoteGag_targetPlayerNameBOTH, Globals_VoteGag.VoteGag_GetVoted[Globals_VoteGag.VoteGag_targetPlayerNameBOTH], Globals_VoteGag.VoteGag_requiredboth);
                        }
                        return HookResult.Continue;
                    }
                }
            }
        }
        return HookResult.Continue;
    }
    private void HandleMenuCT(CCSPlayerController Caller, ChatMenuOption option, int TargetPlayersUserID)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";

        var GetTarget = Utilities.GetPlayerFromUserid(TargetPlayersUserID);
        if(GetTarget == null || !GetTarget.IsValid)return;
        DateTime personDate = DateTime.Now;

        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var TargetPlayerName = GetTarget.PlayerName;
        var TargetPlayerSteamID = GetTarget.SteamID;
        var TargetPlayerTeam = GetTarget.TeamNum;

        string TargerIP = GetTarget.IpAddress?.Split(':')[0] ?? "InvildIpAdress";

        var allCTPlayers = Helper.GetCounterTerroristCount();
        float percentage = Configs.GetConfigData().VoteGag_Percentage;
        int requiredct = (int)Math.Ceiling(allCTPlayers * (percentage / 100.0f));

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_ImmunityGroups) && Globals_VoteGag.VoteGag_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votegag.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        

        if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.vote.same.player"],TargetPlayerName, Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName], requiredct);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteGag_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredct/2)
        {
            var AllCTPlayers = Helper.GetCounterTerroristController();
            foreach (var players in AllCTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid))
                {
                    Globals_VoteGag.VoteGag_ShowMenuCT.Add(steamid, true);
                }
            }
            
            Globals_VoteGag.VoteGag_timerCT = Configs.GetConfigData().VoteGag_CenterMessageAnnouncementTimer;
            Globals_VoteGag.VoteGag_stopwatchCT.Start();
            Globals_VoteGag.VoteGag_targetPlayerNameCT = TargetPlayerName;
            Globals_VoteGag.VoteGag_countingCT = Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName];
            Globals_VoteGag.VoteGag_requiredct = requiredct;
            Globals_VoteGag.VoteGag_targetPlayerIPCT = TargerIP;
            Globals_VoteGag.VoteGag_targetPlayerSTEAMCT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
        {
            var playersct = Helper.GetCounterTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votegag.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votegag.chat.message"], CallerName, TargetPlayerName, Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName], requiredct)
                );
            }
            
            if (Configs.GetConfigData().VoteGag_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredct/2 && !string.IsNullOrEmpty(Localizer!["votegag.announce.halfvotes.chat.message"]))
            {
                playersct.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredct && !string.IsNullOrEmpty(Localizer!["votegag.announce.gaged.successfully.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.gaged.successfully.message"], TargetPlayerName)
                );
            }
            
        }

        if (Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredct)
        {
            
            Json_VoteGag.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteGag_TimeInMins, Configs.GetConfigData().VoteGag_TimeInMins, "Vote Gaged", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Gaged");
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
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Gaged");
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }

            if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
            }
            if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(TargetPlayerSteamID))
            {
                Globals_VoteGag.VoteGag_PlayerGaged.Add(TargetPlayerSteamID, true);
            }
            if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playersct = Helper.GetCounterTerroristController();
                playersct.ForEach(player => 
                {
                    Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
                    Globals_VoteGag.VoteGag_GetVoted[player.PlayerName] = 0;
                    Globals_VoteGag.VoteGag_countingCT = 0;
                    Globals_VoteGag.VoteGag_ShowMenuCT.Remove(player.SteamID);
                });
            }
        }
        MenuManager.CloseActiveMenu(Caller);
    }
    private void HandleMenuT(CCSPlayerController Caller, ChatMenuOption option, int TargetPlayersUserID)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";

        var GetTarget = Utilities.GetPlayerFromUserid(TargetPlayersUserID);
        if(GetTarget == null || !GetTarget.IsValid)return;
        DateTime personDate = DateTime.Now;

        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var TargetPlayerName = GetTarget.PlayerName;
        var TargetPlayerSteamID = GetTarget.SteamID;
        var TargetPlayerTeam = GetTarget.TeamNum;

        string TargerIP = GetTarget.IpAddress?.Split(':')[0] ?? "InvildIpAdress";

        var allTPlayers = Helper.GetTerroristCount();
        float percentage = Configs.GetConfigData().VoteGag_Percentage;
        int requiredt = (int)Math.Ceiling(allTPlayers * (percentage / 100.0f));

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_ImmunityGroups) && Globals_VoteGag.VoteGag_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votegag.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        
        if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.vote.same.player"],TargetPlayerName, Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName], requiredt);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteGag_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredt/2)
        {
            var AllTPlayers = Helper.GetTerroristController();
            foreach (var players in AllTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid))
                {
                    Globals_VoteGag.VoteGag_ShowMenuT.Add(steamid, true);
                }
            }
            
            Globals_VoteGag.VoteGag_timerT = Configs.GetConfigData().VoteGag_CenterMessageAnnouncementTimer;
            Globals_VoteGag.VoteGag_stopwatchT.Start();
            Globals_VoteGag.VoteGag_targetPlayerNameT = TargetPlayerName;
            Globals_VoteGag.VoteGag_countingT = Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName];
            Globals_VoteGag.VoteGag_requiredt = requiredt;
            Globals_VoteGag.VoteGag_targetPlayerIPT = TargerIP;
            Globals_VoteGag.VoteGag_targetPlayerSTEAMT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
        {
            var playerst = Helper.GetTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votegag.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votegag.chat.message"], CallerName, TargetPlayerName, Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName], requiredt)
                );
            }
            
            if (Configs.GetConfigData().VoteGag_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredt/2 && !string.IsNullOrEmpty(Localizer!["votegag.announce.halfvotes.chat.message"]))
            {
                playerst.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredt && !string.IsNullOrEmpty(Localizer!["votegag.announce.gaged.successfully.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.gaged.successfully.message"], TargetPlayerName)
                );
            }
        }

        if (Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredt)
        {
            
            Json_VoteGag.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteGag_TimeInMins, Configs.GetConfigData().VoteGag_TimeInMins, "Vote Gaged", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Gaged");
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
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Gaged");
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }
            if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
            }
            if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(TargetPlayerSteamID))
            {
                Globals_VoteGag.VoteGag_PlayerGaged.Add(TargetPlayerSteamID, true);
            }
            if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playerst = Helper.GetTerroristController();
                playerst.ForEach(player => 
                {
                    Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
                    Globals_VoteGag.VoteGag_GetVoted[player.PlayerName] = 0;
                    Globals_VoteGag.VoteGag_countingT = 0;
                    Globals_VoteGag.VoteGag_ShowMenuT.Remove(player.SteamID);
                });
            }
        }
        MenuManager.CloseActiveMenu(Caller);
    }

    private void HandleMenuALL(CCSPlayerController Caller, ChatMenuOption option, int TargetPlayersUserID)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";

        var GetTarget = Utilities.GetPlayerFromUserid(TargetPlayersUserID);
        if(GetTarget == null || !GetTarget.IsValid)return;
        DateTime personDate = DateTime.Now;

        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var TargetPlayerName = GetTarget.PlayerName;
        var TargetPlayerSteamID = GetTarget.SteamID;
        var TargetPlayerTeam = GetTarget.TeamNum;

        string TargerIP = GetTarget.IpAddress?.Split(':')[0] ?? "InvildIpAdress";

        var allPlayers = Helper.GetAllCount();
        float percentage = Configs.GetConfigData().VoteGag_Percentage;
        int requiredall = (int)Math.Ceiling(allPlayers * (percentage / 100.0f));

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteGag_ImmunityGroups) && Globals_VoteGag.VoteGag_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votegag.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        if (!Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteGag.VoteGag_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            
            Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Add(TargetPlayerName);
            Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] + 1 : 1;
            
        }else if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votegag.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votegag.player.vote.same.player"],TargetPlayerName, Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteGag_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteGag.VoteGag_ShowMenuBOTH.Add(steamid, true);
                }
            }
            
            Globals_VoteGag.VoteGag_timerBOTH = Configs.GetConfigData().VoteGag_CenterMessageAnnouncementTimer;
            Globals_VoteGag.VoteGag_stopwatchBOTH.Start();
            Globals_VoteGag.VoteGag_targetPlayerNameBOTH = TargetPlayerName;
            Globals_VoteGag.VoteGag_countingBoth = Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName];
            Globals_VoteGag.VoteGag_requiredboth = requiredall;
            Globals_VoteGag.VoteGag_targetPlayerIPBOTH = TargerIP;
            Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH = TargetPlayerSteamID;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votegag.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votegag.chat.message"], CallerName, TargetPlayerName, Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteGag_CenterMessageAnnouncementOnHalfVotes && Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votegag.announce.halfvotes.chat.message"]))
        {
            var playerall = Helper.GetAllController();
            playerall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid) || Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votegag.announce.halfvotes.chat.message"]);
            });
        }
        
        if (Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votegag.announce.gaged.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votegag.announce.gaged.successfully.message"], TargetPlayerName);
        }
        

        if (Globals_VoteGag.VoteGag_GetVoted[TargetPlayerName] >= requiredall)
        {
            
            Json_VoteGag.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteGag_TimeInMins, Configs.GetConfigData().VoteGag_TimeInMins, "Vote Gaged", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Gaged");
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
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Gaged");
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Task.Run(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }
            if (!string.IsNullOrEmpty(Localizer!["votegag.player.gaged.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votegag.player.gaged.successfully.message"], Configs.GetConfigData().VoteGag_TimeInMins);
            }
            if (!Globals_VoteGag.VoteGag_PlayerGaged.ContainsKey(TargetPlayerSteamID))
            {
                Globals_VoteGag.VoteGag_PlayerGaged.Add(TargetPlayerSteamID, true);
            }
            if (Globals_VoteGag.VoteGag_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playersall = Helper.GetAllController();
                playersall.ForEach(player => 
                {
                    Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
                    Globals_VoteGag.VoteGag_GetVoted[player.PlayerName] = 0;
                    Globals_VoteGag.VoteGag_countingBoth = 0;
                    Globals_VoteGag.VoteGag_ShowMenuBOTH.Remove(player.SteamID);
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

        if (Globals_VoteGag.VoteGag_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteGag.VoteGag_CallerVotedTo[player])
            {
                if (Globals_VoteGag.VoteGag_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteGag.VoteGag_GetVoted[votedPlayer]--;
                    Globals_VoteGag.VoteGag_countingCT = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteGag.VoteGag_countingCT - 1 : 1;
                    Globals_VoteGag.VoteGag_countingT = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteGag.VoteGag_countingT - 1 : 1;
                    Globals_VoteGag.VoteGag_countingBoth = Globals_VoteGag.VoteGag_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteGag.VoteGag_countingBoth - 1 : 1;
                }
            }
            Globals_VoteGag.VoteGag_CallerVotedTo.Remove(player);
        }

        if (Globals_VoteGag.VoteGag_Disable.ContainsKey(playerid))
        {
            Globals_VoteGag.VoteGag_Disable.Remove(playerid);
            foreach (var allplayers in Helper.GetAllController())
            {
                if(allplayers == null || !allplayers.IsValid)continue;
                var playerssteamid = allplayers.SteamID;
                if (!Globals_VoteGag.VoteGag_Disable.ContainsKey(playerssteamid))
                {
                    Globals_VoteGag.VoteGag_Disabled = false;
                }
            }
        }
        
        Globals_VoteGag.VoteGag_PlayerGaged.Remove(playerid);

        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        if (Configs.GetConfigData().VoteGag_RemoveGagedPlayersOnMapChange)
        {
            Json_VoteGag.RemoveAnyByReason("Vote Gaged", filename);
        }
        Helper.ClearVariablesVoteGag();
    }
}