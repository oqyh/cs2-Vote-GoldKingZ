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

public class VoteMute
{
    private IStringLocalizer? Localizer;
    private string filename = "Mute.json";
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
        var GetPlayerIp = player.IpAddress;
        string[] parts = GetPlayerIp!.Split(':');
        string PlayerIp = parts[0];
        string Reason = "Mute Evasion";

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_DisableItOnJoinTheseGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteMute_DisableItOnJoinTheseGroups))
        {
            if (!Globals_VoteMute.VoteMute_Disable.ContainsKey(playerid))
            {
                Globals_VoteMute.VoteMute_Disable.Add(playerid, true);
                Globals_VoteMute.VoteMute_Disabled = true;
            }
        }

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_ImmunityGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteMute_ImmunityGroups))
        {
            if (!Globals_VoteMute.VoteMute_Immunity.ContainsKey(playerid))
            {
                Globals_VoteMute.VoteMute_Immunity.Add(playerid, true);
            }
        }
        
        Json_VoteMute.PersonData personDataIP = Json_VoteMute.RetrievePersonDataByIp(PlayerIp, Configs.GetConfigData().VoteMute_TimeInMins, filename);
        Json_VoteMute.PersonData personDataID = Json_VoteMute.RetrievePersonDataById(playerid, Configs.GetConfigData().VoteMute_TimeInMins, filename);
        Json_VoteMute.PersonData personDataREASON = Json_VoteMute.RetrievePersonDataByReason(Reason, Configs.GetConfigData().VoteMute_TimeInMins, filename);
        DateTime personDate = DateTime.Now;
        
        if(Json_VoteMute.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteMute_TimeInMins, filename) || Json_VoteMute.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteMute_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteMute_EvasionPunishment && (personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason || personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason))
            {
                Json_VoteMute.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteMute_EvasionPunishmentTimeInMins, "Mute Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Mute Evasion");
                    if(!Directory.Exists(Fpath))
                    {
                        Directory.CreateDirectory(Fpath);
                    }

                    if(!File.Exists(Tpath))
                    {
                        File.Create(Tpath);
                    }

                    try
                    {
                        File.AppendAllLines(Tpath, new[]{replacerlog});
                    }catch
                    {

                    }
                }
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Mute Evasion");
                if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                {
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                    {
                        Server.NextFrame(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                        });
                    }
                }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                {
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                    {
                        Server.NextFrame(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, playerid.ToString(), playername);
                        });
                    }
                }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                {
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                    {
                        Server.NextFrame(() =>
                        {
                            _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, playerid.ToString(), playername);
                        });
                    }
                }
            }
            player.VoiceFlags = VoiceFlags.Muted;
            VoteGoldKingZ pluginInstance = new VoteGoldKingZ();
            Server.NextFrame(() =>
            {
                pluginInstance.AddTimer(6.30f, () =>
                {
                    if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
                    {
                        Helper.AdvancedPrintToChat(player, Localizer!["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
                    }
                    if (pluginInstance is IDisposable disposableInstance)
                    {
                        disposableInstance.Dispose();
                    }
                });
            });
            
                
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_CommandsToVote) || @event == null)return HookResult.Continue;
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
        string[] CenterMenuCommands = Configs.GetConfigData().VoteMute_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteMuteMenu = new ChatMenu(Localizer!["votemute.menu.name"]);            
            if(Configs.GetConfigData().VoteMute_TeamOnly)
            {
                if(CallerTeam == (byte)CsTeam.CounterTerrorist)
                {
                    var AllCTPlayers = Helper.GetCounterTerroristController();
                    var AllCTPlayersCount = Helper.GetCounterTerroristCount();
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_DisableItOnJoinTheseGroups) && Globals_VoteMute.VoteMute_Disabled)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.is.disabled"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.is.disabled"]);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount < Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.minimum.needed"],Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount >= Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllCTPlayers)
                        {
                            if(Caller == players)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteMuteMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuCT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
                if(CallerTeam == (byte)CsTeam.Terrorist)
                {
                    var AllTPlayers = Helper.GetTerroristController();
                    var AllTPlayersCount = Helper.GetTerroristCount();
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_DisableItOnJoinTheseGroups) && Globals_VoteMute.VoteMute_Disabled)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.is.disabled"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.is.disabled"]);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount < Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.minimum.needed"],Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount >= Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllTPlayers)
                        {
                            if(Caller == players)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteMuteMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
            }else
            {
                var AllPlayers = Helper.GetAllController();
                var AllPlayersCount = Helper.GetAllCount();
                if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_DisableItOnJoinTheseGroups) && Globals_VoteMute.VoteMute_Disabled)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votemute.player.is.disabled"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.is.disabled"]);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount < Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votemute.minimum.needed"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.minimum.needed"],Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount >= Configs.GetConfigData().VoteMute_StartOnMinimumOfXPlayers)
                {
                    foreach (var players in AllPlayers)
                    {
                        if(Caller == players)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteMuteMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuALL(Caller, option, TargetPlayersUserID));
                    }
                }
            }
            VoteMuteMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteMuteMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteMute_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteMute_TeamOnly)
            {
                if (Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteMute.VoteMute_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameCT)
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Add(Globals_VoteMute.VoteMute_targetPlayerNameCT);
                        Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameCT) ? Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT] + 1 : 1;
                        Globals_VoteMute.VoteMute_countingCT = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameCT) ? Globals_VoteMute.VoteMute_countingCT + 1 : 1;
                        
                    }else if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.player.vote.same.yes"],Globals_VoteMute.VoteMute_targetPlayerNameCT, Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT], Globals_VoteMute.VoteMute_requiredct);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT] >= Globals_VoteMute.VoteMute_requiredct)
                    {
                        
                        Json_VoteMute.SaveToJsonFile(Globals_VoteMute.VoteMute_targetPlayerSTEAMCT, Globals_VoteMute.VoteMute_targetPlayerNameCT, Globals_VoteMute.VoteMute_targetPlayerIPCT!.ToString(), personDate, Configs.GetConfigData().VoteMute_TimeInMins, "Vote Muted", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteMute.VoteMute_targetPlayerNameCT, Globals_VoteMute.VoteMute_targetPlayerSTEAMCT.ToString(), Globals_VoteMute.VoteMute_targetPlayerIPCT!.ToString(), "Vote Muted");
                            if(!Directory.Exists(Fpath))
                            {
                                Directory.CreateDirectory(Fpath);
                            }

                            if(!File.Exists(Tpath))
                            {
                                File.Create(Tpath);
                            }

                            try
                            {
                                File.AppendAllLines(Tpath, new[]{replacerlog});
                            }catch
                            {

                            }
                        }
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteMute.VoteMute_targetPlayerNameCT, Globals_VoteMute.VoteMute_targetPlayerSTEAMCT.ToString(), Globals_VoteMute.VoteMute_targetPlayerIPCT!.ToString(), "Vote Muted");
                        if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteMute.VoteMute_targetPlayerSTEAMCT.ToString(), Globals_VoteMute.VoteMute_targetPlayerNameCT);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteMute.VoteMute_targetPlayerSTEAMCT.ToString(), Globals_VoteMute.VoteMute_targetPlayerNameCT);
                                });
                            }
                        }
                        
                        var playerctall = Helper.GetCounterTerroristController();
                        playerctall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteMute.VoteMute_targetPlayerSTEAMCT)
                            {
                                player.VoiceFlags = VoiceFlags.Muted;
                                if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votemute.announce.muted.successfully.message"]))
                        {
                            playerctall.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.muted.successfully.message"], Globals_VoteMute.VoteMute_targetPlayerNameCT)
                            );
                        }

                        if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameCT))
                        {
                            playerctall.ForEach(player => 
                            {
                                Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
                                Globals_VoteMute.VoteMute_GetVoted[player.PlayerName] = 0;
                                Globals_VoteMute.VoteMute_countingCT = 0;
                                Globals_VoteMute.VoteMute_ShowMenuCT.Remove(player.SteamID);
                            });
                        }
                    }
                }
                if (Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteMute.VoteMute_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameT)
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Add(Globals_VoteMute.VoteMute_targetPlayerNameT);
                        Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameT) ? Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT] + 1 : 1;
                        Globals_VoteMute.VoteMute_countingT = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameT) ? Globals_VoteMute.VoteMute_countingT + 1 : 1;
                        
                    }else if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.player.vote.same.yes"],Globals_VoteMute.VoteMute_targetPlayerNameT, Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT], Globals_VoteMute.VoteMute_requiredt);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT] >= Globals_VoteMute.VoteMute_requiredt)
                    {
                        
                        Json_VoteMute.SaveToJsonFile(Globals_VoteMute.VoteMute_targetPlayerSTEAMT, Globals_VoteMute.VoteMute_targetPlayerNameT, Globals_VoteMute.VoteMute_targetPlayerIPT!.ToString(), personDate, Configs.GetConfigData().VoteMute_TimeInMins, "Vote Muted", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteMute.VoteMute_targetPlayerNameT, Globals_VoteMute.VoteMute_targetPlayerSTEAMT.ToString(), Globals_VoteMute.VoteMute_targetPlayerIPT!.ToString(), "Vote Muted");
                            if(!Directory.Exists(Fpath))
                            {
                                Directory.CreateDirectory(Fpath);
                            }

                            if(!File.Exists(Tpath))
                            {
                                File.Create(Tpath);
                            }

                            try
                            {
                                File.AppendAllLines(Tpath, new[]{replacerlog});
                            }catch
                            {

                            }
                        }
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteMute.VoteMute_targetPlayerNameT, Globals_VoteMute.VoteMute_targetPlayerSTEAMT.ToString(), Globals_VoteMute.VoteMute_targetPlayerIPT!.ToString(), "Vote Muted");
                        if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteMute.VoteMute_targetPlayerSTEAMT.ToString(), Globals_VoteMute.VoteMute_targetPlayerNameT);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteMute.VoteMute_targetPlayerSTEAMT.ToString(), Globals_VoteMute.VoteMute_targetPlayerNameT);
                                });
                            }
                        }
                        
                        
                        var playertall = Helper.GetTerroristController();
                        playertall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteMute.VoteMute_targetPlayerSTEAMT)
                            {
                                player.VoiceFlags = VoiceFlags.Muted;
                                if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votemute.announce.muted.successfully.message"]))
                        {
                            playertall.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.muted.successfully.message"], Globals_VoteMute.VoteMute_targetPlayerNameT)
                            );
                        }
                        if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameT))
                        {
                            playertall.ForEach(player => 
                            {
                                Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
                                Globals_VoteMute.VoteMute_GetVoted[player.PlayerName] = 0;
                                Globals_VoteMute.VoteMute_countingT = 0;
                                Globals_VoteMute.VoteMute_ShowMenuT.Remove(player.SteamID);
                            });
                        }
                    }
                }
            }else
            {
                if (Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteMute.VoteMute_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameBOTH)
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Add(Globals_VoteMute.VoteMute_targetPlayerNameBOTH);
                        Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) ? Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH] + 1 : 1;
                        Globals_VoteMute.VoteMute_countingBoth = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) ? Globals_VoteMute.VoteMute_countingBoth + 1 : 1;
                        
                    }else if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.player.vote.same.yes"],Globals_VoteMute.VoteMute_targetPlayerNameBOTH, Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH], Globals_VoteMute.VoteMute_requiredboth);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH] >= Globals_VoteMute.VoteMute_requiredboth)
                    {
                        
                        Json_VoteMute.SaveToJsonFile(Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH, Globals_VoteMute.VoteMute_targetPlayerNameBOTH, Globals_VoteMute.VoteMute_targetPlayerIPBOTH!.ToString(), personDate, Configs.GetConfigData().VoteMute_TimeInMins, "Vote Muted", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteMute.VoteMute_targetPlayerNameBOTH, Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH.ToString(), Globals_VoteMute.VoteMute_targetPlayerIPBOTH!.ToString(), "Vote Muted");
                            if(!Directory.Exists(Fpath))
                            {
                                Directory.CreateDirectory(Fpath);
                            }

                            if(!File.Exists(Tpath))
                            {
                                File.Create(Tpath);
                            }

                            try
                            {
                                File.AppendAllLines(Tpath, new[]{replacerlog});
                            }catch
                            {

                            }
                        }
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteMute.VoteMute_targetPlayerNameBOTH, Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH.ToString(), Globals_VoteMute.VoteMute_targetPlayerIPBOTH!.ToString(), "Vote Muted");
                        if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH.ToString(), Globals_VoteMute.VoteMute_targetPlayerNameBOTH);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Server.NextFrame(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH.ToString(), Globals_VoteMute.VoteMute_targetPlayerNameBOTH);
                                });
                            }
                        }
                        
                        
                        var playerbothall = Helper.GetAllController();
                        playerbothall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH)
                            {
                                player.VoiceFlags = VoiceFlags.Muted;
                                if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votemute.announce.muted.successfully.message"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["votemute.announce.muted.successfully.message"], Globals_VoteMute.VoteMute_targetPlayerNameBOTH);
                        }
                        if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameBOTH))
                        {
                            playerbothall.ForEach(player => 
                            {
                                Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
                                Globals_VoteMute.VoteMute_GetVoted[player.PlayerName] = 0;
                                Globals_VoteMute.VoteMute_countingBoth = 0;
                                Globals_VoteMute.VoteMute_ShowMenuBOTH.Remove(player.SteamID);
                            });
                        }
                    }
                }
            }
        }
        string[] Refuse = Configs.GetConfigData().VoteMute_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteMute_TeamOnly)
            {
                if (Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteMute.VoteMute_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameCT)
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Remove(Globals_VoteMute.VoteMute_targetPlayerNameCT);
                        Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameCT) ? Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT] - 1 : 1;
                        Globals_VoteMute.VoteMute_countingCT = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameCT) ? Globals_VoteMute.VoteMute_countingCT - 1 : 1;
                        
                    }else if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.player.vote.same.no"],Globals_VoteMute.VoteMute_targetPlayerNameCT, Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameCT], Globals_VoteMute.VoteMute_requiredct);
                        }
                        return HookResult.Continue;
                    }
                    
                }
                if (Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteMute.VoteMute_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameT)
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Remove(Globals_VoteMute.VoteMute_targetPlayerNameT);
                        Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameT) ? Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT] - 1 : 1;
                        Globals_VoteMute.VoteMute_countingT = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameT) ? Globals_VoteMute.VoteMute_countingT - 1 : 1;
                        
                    }else if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameT) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.player.vote.same.no"],Globals_VoteMute.VoteMute_targetPlayerNameT, Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameT], Globals_VoteMute.VoteMute_requiredt);
                        }
                        return HookResult.Continue;
                    }
                }
            }else
            {
                if (Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteMute.VoteMute_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameBOTH)
                    {
                        Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Remove(Globals_VoteMute.VoteMute_targetPlayerNameBOTH);
                        Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) ? Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH] - 1 : 1;
                        Globals_VoteMute.VoteMute_countingBoth = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) ? Globals_VoteMute.VoteMute_countingBoth - 1 : 1;
                        
                    }else if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(Globals_VoteMute.VoteMute_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteMute.VoteMute_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votemute.player.vote.same.no"],Globals_VoteMute.VoteMute_targetPlayerNameBOTH, Globals_VoteMute.VoteMute_GetVoted[Globals_VoteMute.VoteMute_targetPlayerNameBOTH], Globals_VoteMute.VoteMute_requiredboth);
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

        var GetTargerIP = GetTarget.IpAddress;
        string[] parts = GetTargerIP!.Split(':');
        string TargerIP = parts[0];

        double xPercentage = Configs.GetConfigData().VoteMute_Percentage / 10.0;
        var CountCT = Helper.GetCounterTerroristCount();
        var requiredct = (int)Math.Ceiling(CountCT * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_ImmunityGroups) && Globals_VoteMute.VoteMute_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votemute.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        

        if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.vote.same.player"],TargetPlayerName, Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName], requiredct);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteMute_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredct/2)
        {
            var AllCTPlayers = Helper.GetCounterTerroristController();
            foreach (var players in AllCTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid))
                {
                    Globals_VoteMute.VoteMute_ShowMenuCT.Add(steamid, true);
                }
            }
            
            Globals_VoteMute.VoteMute_timerCT = Configs.GetConfigData().VoteMute_CenterMessageAnnouncementTimer;
            Globals_VoteMute.VoteMute_stopwatchCT.Start();
            Globals_VoteMute.VoteMute_targetPlayerNameCT = TargetPlayerName;
            Globals_VoteMute.VoteMute_countingCT = Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName];
            Globals_VoteMute.VoteMute_requiredct = requiredct;
            Globals_VoteMute.VoteMute_targetPlayerIPCT = TargerIP;
            Globals_VoteMute.VoteMute_targetPlayerSTEAMCT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
        {
            var playersct = Helper.GetCounterTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votemute.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votemute.chat.message"], CallerName, TargetPlayerName, Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName], requiredct)
                );
            }
            
            if (Configs.GetConfigData().VoteMute_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredct/2 && !string.IsNullOrEmpty(Localizer!["votemute.announce.halfvotes.chat.message"]))
            {
                playersct.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredct && !string.IsNullOrEmpty(Localizer!["votemute.announce.muted.successfully.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.muted.successfully.message"], TargetPlayerName)
                );
            }
            
        }

        if (Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredct)
        {
            
            Json_VoteMute.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteMute_TimeInMins, "Vote Muted", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Muted");
                if(!Directory.Exists(Fpath))
                {
                    Directory.CreateDirectory(Fpath);
                }

                if(!File.Exists(Tpath))
                {
                    File.Create(Tpath);
                }

                try
                {
                    File.AppendAllLines(Tpath, new[]{replacerlog});
                }catch
                {

                }
            }
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Muted");
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }

            if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
            }
            GetTarget.VoiceFlags = VoiceFlags.Muted;
            if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playersct = Helper.GetCounterTerroristController();
                playersct.ForEach(player => 
                {
                    Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
                    Globals_VoteMute.VoteMute_GetVoted[player.PlayerName] = 0;
                    Globals_VoteMute.VoteMute_countingCT = 0;
                    Globals_VoteMute.VoteMute_ShowMenuCT.Remove(player.SteamID);
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

        var GetTargerIP = GetTarget.IpAddress;
        string[] parts = GetTargerIP!.Split(':');
        string TargerIP = parts[0];

        double xPercentage = Configs.GetConfigData().VoteMute_Percentage / 10.0;
        var CountT = Helper.GetTerroristCount();
        var requiredt = (int)Math.Ceiling(CountT * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_ImmunityGroups) && Globals_VoteMute.VoteMute_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votemute.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        
        if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.vote.same.player"],TargetPlayerName, Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName], requiredt);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteMute_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredt/2)
        {
            var AllTPlayers = Helper.GetTerroristController();
            foreach (var players in AllTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid))
                {
                    Globals_VoteMute.VoteMute_ShowMenuT.Add(steamid, true);
                }
            }
            
            Globals_VoteMute.VoteMute_timerT = Configs.GetConfigData().VoteMute_CenterMessageAnnouncementTimer;
            Globals_VoteMute.VoteMute_stopwatchT.Start();
            Globals_VoteMute.VoteMute_targetPlayerNameT = TargetPlayerName;
            Globals_VoteMute.VoteMute_countingT = Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName];
            Globals_VoteMute.VoteMute_requiredt = requiredt;
            Globals_VoteMute.VoteMute_targetPlayerIPT = TargerIP;
            Globals_VoteMute.VoteMute_targetPlayerSTEAMT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
        {
            var playerst = Helper.GetTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votemute.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votemute.chat.message"], CallerName, TargetPlayerName, Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName], requiredt)
                );
            }
            
            if (Configs.GetConfigData().VoteMute_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredt/2 && !string.IsNullOrEmpty(Localizer!["votemute.announce.halfvotes.chat.message"]))
            {
                playerst.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredt && !string.IsNullOrEmpty(Localizer!["votemute.announce.muted.successfully.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.muted.successfully.message"], TargetPlayerName)
                );
            }
        }

        if (Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredt)
        {
            
            Json_VoteMute.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteMute_TimeInMins, "Vote Muted", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Muted");
                if(!Directory.Exists(Fpath))
                {
                    Directory.CreateDirectory(Fpath);
                }

                if(!File.Exists(Tpath))
                {
                    File.Create(Tpath);
                }

                try
                {
                    File.AppendAllLines(Tpath, new[]{replacerlog});
                }catch
                {

                }
            }
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Muted");
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }
            if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
            }
            GetTarget.VoiceFlags = VoiceFlags.Muted;
            if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playerst = Helper.GetTerroristController();
                playerst.ForEach(player => 
                {
                    Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
                    Globals_VoteMute.VoteMute_GetVoted[player.PlayerName] = 0;
                    Globals_VoteMute.VoteMute_countingT = 0;
                    Globals_VoteMute.VoteMute_ShowMenuT.Remove(player.SteamID);
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

        var GetTargerIP = GetTarget.IpAddress;
        string[] parts = GetTargerIP!.Split(':');
        string TargerIP = parts[0];

        double xPercentage = Configs.GetConfigData().VoteMute_Percentage / 10.0;
        var Countall = Helper.GetAllCount();
        var requiredall = (int)Math.Ceiling(Countall * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteMute_ImmunityGroups) && Globals_VoteMute.VoteMute_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votemute.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        if (!Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteMute.VoteMute_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            
            Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Add(TargetPlayerName);
            Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] + 1 : 1;
            
        }else if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votemute.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votemute.player.vote.same.player"],TargetPlayerName, Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteMute_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteMute.VoteMute_ShowMenuBOTH.Add(steamid, true);
                }
            }
            
            Globals_VoteMute.VoteMute_timerBOTH = Configs.GetConfigData().VoteMute_CenterMessageAnnouncementTimer;
            Globals_VoteMute.VoteMute_stopwatchBOTH.Start();
            Globals_VoteMute.VoteMute_targetPlayerNameBOTH = TargetPlayerName;
            Globals_VoteMute.VoteMute_countingBoth = Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName];
            Globals_VoteMute.VoteMute_requiredboth = requiredall;
            Globals_VoteMute.VoteMute_targetPlayerIPBOTH = TargerIP;
            Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH = TargetPlayerSteamID;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votemute.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votemute.chat.message"], CallerName, TargetPlayerName, Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteMute_CenterMessageAnnouncementOnHalfVotes && Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votemute.announce.halfvotes.chat.message"]))
        {
            var playerall = Helper.GetAllController();
            playerall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votemute.announce.halfvotes.chat.message"]);
            });
        }
        
        if (Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votemute.announce.muted.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votemute.announce.muted.successfully.message"], TargetPlayerName);
        }
        

        if (Globals_VoteMute.VoteMute_GetVoted[TargetPlayerName] >= requiredall)
        {
            
            Json_VoteMute.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteMute_TimeInMins, "Vote Muted", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Muted");
                if(!Directory.Exists(Fpath))
                {
                    Directory.CreateDirectory(Fpath);
                }

                if(!File.Exists(Tpath))
                {
                    File.Create(Tpath);
                }

                try
                {
                    File.AppendAllLines(Tpath, new[]{replacerlog});
                }catch
                {

                }
            }
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Muted");
            if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 1)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNormal(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 2)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
            {
                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                {
                    Server.NextFrame(() =>
                    {
                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, TargetPlayerSteamID.ToString(), TargetPlayerName);
                    });
                }
            }
            if (!string.IsNullOrEmpty(Localizer!["votemute.player.muted.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votemute.player.muted.successfully.message"], Configs.GetConfigData().VoteMute_TimeInMins);
            }
            GetTarget.VoiceFlags = VoiceFlags.Muted;
            if (Globals_VoteMute.VoteMute_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playersall = Helper.GetAllController();
                playersall.ForEach(player => 
                {
                    Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
                    Globals_VoteMute.VoteMute_GetVoted[player.PlayerName] = 0;
                    Globals_VoteMute.VoteMute_countingBoth = 0;
                    Globals_VoteMute.VoteMute_ShowMenuBOTH.Remove(player.SteamID);
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

        if (Globals_VoteMute.VoteMute_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteMute.VoteMute_CallerVotedTo[player])
            {
                if (Globals_VoteMute.VoteMute_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteMute.VoteMute_GetVoted[votedPlayer]--;
                    Globals_VoteMute.VoteMute_countingCT = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteMute.VoteMute_countingCT - 1 : 1;
                    Globals_VoteMute.VoteMute_countingT = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteMute.VoteMute_countingT - 1 : 1;
                    Globals_VoteMute.VoteMute_countingBoth = Globals_VoteMute.VoteMute_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteMute.VoteMute_countingBoth - 1 : 1;
                }
            }
            Globals_VoteMute.VoteMute_CallerVotedTo.Remove(player);
        }

        if (Globals_VoteMute.VoteMute_Disable.ContainsKey(playerid))
        {
            Globals_VoteMute.VoteMute_Disable.Remove(playerid);
            foreach (var allplayers in Helper.GetAllController())
            {
                if(allplayers == null || !allplayers.IsValid)continue;
                var playerssteamid = allplayers.SteamID;
                if (!Globals_VoteMute.VoteMute_Disable.ContainsKey(playerssteamid))
                {
                    Globals_VoteMute.VoteMute_Disabled = false;
                }
            }
        }
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        if (Configs.GetConfigData().VoteMute_RemoveMutedPlayersOnMapChange)
        {
            Json_VoteMute.RemoveAnyByReason("Vote Muted", filename);
        }
        Helper.ClearVariablesVoteMute();
    }
}