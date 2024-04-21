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

public class VoteSilent
{
    private IStringLocalizer? Localizer;
    private string filename = "Silent.json";
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
        string Reason = "Silent Evasion";

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_DisableItOnJoinTheseGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteSilent_DisableItOnJoinTheseGroups))
        {
            if (!Globals_VoteSilent.VoteSilent_Disable.ContainsKey(playerid))
            {
                Globals_VoteSilent.VoteSilent_Disable.Add(playerid, true);
                Globals_VoteSilent.VoteSilent_Disabled = true;
            }
        }

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_ImmunityGroups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteSilent_ImmunityGroups))
        {
            if (!Globals_VoteSilent.VoteSilent_Immunity.ContainsKey(playerid))
            {
                Globals_VoteSilent.VoteSilent_Immunity.Add(playerid, true);
            }
        }
        
        Json_VoteSilent.PersonData personDataIP = Json_VoteSilent.RetrievePersonDataByIp(PlayerIp, Configs.GetConfigData().VoteSilent_TimeInMins, filename);
        Json_VoteSilent.PersonData personDataID = Json_VoteSilent.RetrievePersonDataById(playerid, Configs.GetConfigData().VoteSilent_TimeInMins, filename);
        Json_VoteSilent.PersonData personDataREASON = Json_VoteSilent.RetrievePersonDataByReason(Reason, Configs.GetConfigData().VoteSilent_TimeInMins, filename);
        DateTime personDate = DateTime.Now;
        
        if(Json_VoteSilent.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteSilent_TimeInMins, filename) || Json_VoteSilent.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteSilent_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteSilent_EvasionPunishment && (personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason || personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason))
            {
                Json_VoteSilent.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteSilent_EvasionPunishmentTimeInMins, Configs.GetConfigData().VoteSilent_EvasionPunishmentTimeInMins, "Silent Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Silent Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Silent Evasion");
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
            player.VoiceFlags = VoiceFlags.Muted;
            if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(playerid))
            {
                Globals_VoteSilent.VoteSilent_PlayerGaged.Add(playerid, true);
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
                    if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
                    {
                        Helper.AdvancedPrintToChat(player, Localizer!["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
                    }
                }, TimerFlags.STOP_ON_MAPCHANGE);
            });
            
                
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_CommandsToVote) || @event == null)return HookResult.Continue;
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
        string[] CenterMenuCommands = Configs.GetConfigData().VoteSilent_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteSilentMenu = new ChatMenu(Localizer!["votesilent.menu.name"]);            
            if(Configs.GetConfigData().VoteSilent_TeamOnly)
            {
                if(CallerTeam == (byte)CsTeam.CounterTerrorist)
                {
                    var AllCTPlayers = Helper.GetCounterTerroristController();
                    var AllCTPlayersCount = Helper.GetCounterTerroristCount();
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_DisableItOnJoinTheseGroups) && Globals_VoteSilent.VoteSilent_Disabled)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.is.disabled"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.is.disabled"]);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount < Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.minimum.needed"],Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount >= Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllCTPlayers)
                        {
                            if(Caller == players || players == null || !players.IsValid)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteSilentMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuCT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
                if(CallerTeam == (byte)CsTeam.Terrorist)
                {
                    var AllTPlayers = Helper.GetTerroristController();
                    var AllTPlayersCount = Helper.GetTerroristCount();
                    if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_DisableItOnJoinTheseGroups) && Globals_VoteSilent.VoteSilent_Disabled)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.is.disabled"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.is.disabled"]);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount < Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.minimum.needed"],Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount >= Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllTPlayers)
                        {
                            if(Caller == players || players == null || !players.IsValid)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteSilentMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
            }else
            {
                var AllPlayers = Helper.GetAllController();
                var AllPlayersCount = Helper.GetAllCount();
                if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_DisableItOnJoinTheseGroups) && Globals_VoteSilent.VoteSilent_Disabled)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votesilent.player.is.disabled"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.is.disabled"]);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount < Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votesilent.minimum.needed"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.minimum.needed"],Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount >= Configs.GetConfigData().VoteSilent_StartOnMinimumOfXPlayers)
                {
                    foreach (var players in AllPlayers)
                    {
                        if(Caller == players || players == null || !players.IsValid)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteSilentMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuALL(Caller, option, TargetPlayersUserID));
                    }
                }
            }
            VoteSilentMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteSilentMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteSilent_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteSilent_TeamOnly)
            {
                if (Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteSilent.VoteSilent_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameCT)
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Add(Globals_VoteSilent.VoteSilent_targetPlayerNameCT);
                        Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) ? Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT] + 1 : 1;
                        Globals_VoteSilent.VoteSilent_countingCT = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) ? Globals_VoteSilent.VoteSilent_countingCT + 1 : 1;
                        
                    }else if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.player.vote.same.yes"],Globals_VoteSilent.VoteSilent_targetPlayerNameCT, Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT], Globals_VoteSilent.VoteSilent_requiredct);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT] >= Globals_VoteSilent.VoteSilent_requiredct)
                    {
                        
                        Json_VoteSilent.SaveToJsonFile(Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT, Globals_VoteSilent.VoteSilent_targetPlayerNameCT, Globals_VoteSilent.VoteSilent_targetPlayerIPCT!.ToString(), personDate, Configs.GetConfigData().VoteSilent_TimeInMins, Configs.GetConfigData().VoteSilent_TimeInMins, "Vote Silented", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteSilent.VoteSilent_targetPlayerNameCT, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerIPCT!.ToString(), "Vote Silented");
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
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, Globals_VoteSilent.VoteSilent_targetPlayerNameCT, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerIPCT!.ToString(), "Vote Silented");
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
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerNameCT);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerNameCT);
                                });
                            }
                        }
                        
                        var playerctall = Helper.GetCounterTerroristController();
                        playerctall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT)
                            {
                                player.VoiceFlags = VoiceFlags.Muted;
                                if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(steamid))
                                {
                                    Globals_VoteSilent.VoteSilent_PlayerGaged.Add(steamid, true);
                                }
                                if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votesilent.announce.silented.successfully.message"]))
                        {
                            playerctall.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.silented.successfully.message"], Globals_VoteSilent.VoteSilent_targetPlayerNameCT)
                            );
                        }

                        if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameCT))
                        {
                            playerctall.ForEach(player => 
                            {
                                Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
                                Globals_VoteSilent.VoteSilent_GetVoted[player.PlayerName] = 0;
                                Globals_VoteSilent.VoteSilent_countingCT = 0;
                                Globals_VoteSilent.VoteSilent_ShowMenuCT.Remove(player.SteamID);
                            });
                        }
                    }
                }
                if (Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteSilent.VoteSilent_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameT)
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Add(Globals_VoteSilent.VoteSilent_targetPlayerNameT);
                        Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameT) ? Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT] + 1 : 1;
                        Globals_VoteSilent.VoteSilent_countingT = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameT) ? Globals_VoteSilent.VoteSilent_countingT + 1 : 1;
                        
                    }else if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.player.vote.same.yes"],Globals_VoteSilent.VoteSilent_targetPlayerNameT, Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT], Globals_VoteSilent.VoteSilent_requiredt);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT] >= Globals_VoteSilent.VoteSilent_requiredt)
                    {
                        
                        Json_VoteSilent.SaveToJsonFile(Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT, Globals_VoteSilent.VoteSilent_targetPlayerNameT, Globals_VoteSilent.VoteSilent_targetPlayerIPT!.ToString(), personDate, Configs.GetConfigData().VoteSilent_TimeInMins, Configs.GetConfigData().VoteSilent_TimeInMins, "Vote Silented", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteSilent.VoteSilent_targetPlayerNameT, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerIPT!.ToString(), "Vote Silented");
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
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, Globals_VoteSilent.VoteSilent_targetPlayerNameT, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerIPT!.ToString(), "Vote Silented");
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
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerNameT);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerNameT);
                                });
                            }
                        }
                        
                        
                        var playertall = Helper.GetTerroristController();
                        playertall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT)
                            {
                                player.VoiceFlags = VoiceFlags.Muted;
                                if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(steamid))
                                {
                                    Globals_VoteSilent.VoteSilent_PlayerGaged.Add(steamid, true);
                                }
                                if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votesilent.announce.silented.successfully.message"]))
                        {
                            playertall.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.silented.successfully.message"], Globals_VoteSilent.VoteSilent_targetPlayerNameT)
                            );
                        }
                        if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameT))
                        {
                            playertall.ForEach(player => 
                            {
                                Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
                                Globals_VoteSilent.VoteSilent_GetVoted[player.PlayerName] = 0;
                                Globals_VoteSilent.VoteSilent_countingT = 0;
                                Globals_VoteSilent.VoteSilent_ShowMenuT.Remove(player.SteamID);
                            });
                        }
                    }
                }
            }else
            {
                if (Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteSilent.VoteSilent_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH)
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Add(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH);
                        Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) ? Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH] + 1 : 1;
                        Globals_VoteSilent.VoteSilent_countingBoth = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) ? Globals_VoteSilent.VoteSilent_countingBoth + 1 : 1;
                        
                    }else if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.player.vote.same.yes"],Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH, Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH], Globals_VoteSilent.VoteSilent_requiredboth);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH] >= Globals_VoteSilent.VoteSilent_requiredboth)
                    {
                        
                        Json_VoteSilent.SaveToJsonFile(Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH, Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH, Globals_VoteSilent.VoteSilent_targetPlayerIPBOTH!.ToString(), personDate, Configs.GetConfigData().VoteSilent_TimeInMins, Configs.GetConfigData().VoteSilent_TimeInMins, "Vote Silented", filename);

                        if(Configs.GetConfigData().Log_SendLogToText)
                        {
                            var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerIPBOTH!.ToString(), "Vote Silented");
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
                        var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerIPBOTH!.ToString(), "Vote Silented");
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
                                    _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH);
                                });
                            }
                        }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                        {
                            if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                            {
                                Task.Run(() =>
                                {
                                    _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH.ToString(), Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH);
                                });
                            }
                        }
                        
                        
                        var playerbothall = Helper.GetAllController();
                        playerbothall.ForEach(player => 
                        {
                            var steamid = player.SteamID;
                            if(steamid == Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH)
                            {
                                player.VoiceFlags = VoiceFlags.Muted;
                                if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(steamid))
                                {
                                    Globals_VoteSilent.VoteSilent_PlayerGaged.Add(steamid, true);
                                }
                                if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
                                {
                                    Helper.AdvancedPrintToChat(player, Localizer["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(Localizer!["votesilent.announce.silented.successfully.message"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["votesilent.announce.silented.successfully.message"], Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH);
                        }
                        if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH))
                        {
                            playerbothall.ForEach(player => 
                            {
                                Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
                                Globals_VoteSilent.VoteSilent_GetVoted[player.PlayerName] = 0;
                                Globals_VoteSilent.VoteSilent_countingBoth = 0;
                                Globals_VoteSilent.VoteSilent_ShowMenuBOTH.Remove(player.SteamID);
                            });
                        }
                    }
                }
            }
        }
        string[] Refuse = Configs.GetConfigData().VoteSilent_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteSilent_TeamOnly)
            {
                if (Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteSilent.VoteSilent_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameCT)
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Remove(Globals_VoteSilent.VoteSilent_targetPlayerNameCT);
                        Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) ? Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT] - 1 : 1;
                        Globals_VoteSilent.VoteSilent_countingCT = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) ? Globals_VoteSilent.VoteSilent_countingCT - 1 : 1;
                        
                    }else if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.player.vote.same.no"],Globals_VoteSilent.VoteSilent_targetPlayerNameCT, Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameCT], Globals_VoteSilent.VoteSilent_requiredct);
                        }
                        return HookResult.Continue;
                    }
                    
                }
                if (Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteSilent.VoteSilent_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameT)
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Remove(Globals_VoteSilent.VoteSilent_targetPlayerNameT);
                        Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameT) ? Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT] - 1 : 1;
                        Globals_VoteSilent.VoteSilent_countingT = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameT) ? Globals_VoteSilent.VoteSilent_countingT - 1 : 1;
                        
                    }else if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameT) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.player.vote.same.no"],Globals_VoteSilent.VoteSilent_targetPlayerNameT, Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameT], Globals_VoteSilent.VoteSilent_requiredt);
                        }
                        return HookResult.Continue;
                    }
                }
            }else
            {
                if (Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteSilent.VoteSilent_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH)
                    {
                        Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Remove(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH);
                        Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) ? Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH] - 1 : 1;
                        Globals_VoteSilent.VoteSilent_countingBoth = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) ? Globals_VoteSilent.VoteSilent_countingBoth - 1 : 1;
                        
                    }else if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votesilent.player.vote.same.no"],Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH, Globals_VoteSilent.VoteSilent_GetVoted[Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH], Globals_VoteSilent.VoteSilent_requiredboth);
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
        float percentage = Configs.GetConfigData().VoteSilent_Percentage;
        int requiredct = (int)Math.Ceiling(allCTPlayers * (percentage / 100.0f));

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_ImmunityGroups) && Globals_VoteSilent.VoteSilent_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        

        if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.vote.same.player"],TargetPlayerName, Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName], requiredct);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementOnHalfVotes && Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredct/2)
        {
            var AllCTPlayers = Helper.GetCounterTerroristController();
            foreach (var players in AllCTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(steamid))
                {
                    Globals_VoteSilent.VoteSilent_ShowMenuCT.Add(steamid, true);
                }
            }
            
            Globals_VoteSilent.VoteSilent_timerCT = Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementTimer;
            Globals_VoteSilent.VoteSilent_stopwatchCT.Start();
            Globals_VoteSilent.VoteSilent_targetPlayerNameCT = TargetPlayerName;
            Globals_VoteSilent.VoteSilent_countingCT = Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName];
            Globals_VoteSilent.VoteSilent_requiredct = requiredct;
            Globals_VoteSilent.VoteSilent_targetPlayerIPCT = TargerIP;
            Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
        {
            var playersct = Helper.GetCounterTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votesilent.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votesilent.chat.message"], CallerName, TargetPlayerName, Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName], requiredct)
                );
            }
            
            if (Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementOnHalfVotes && Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredct/2 && !string.IsNullOrEmpty(Localizer!["votesilent.announce.halfvotes.chat.message"]))
            {
                playersct.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredct && !string.IsNullOrEmpty(Localizer!["votesilent.announce.silented.successfully.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.silented.successfully.message"], TargetPlayerName)
                );
            }
            
        }

        if (Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredct)
        {
            
            Json_VoteSilent.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteSilent_TimeInMins, Configs.GetConfigData().VoteSilent_TimeInMins, "Vote Silented", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Silented");
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
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Silented");
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

            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
            }
            GetTarget.VoiceFlags = VoiceFlags.Muted;
            if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(TargetPlayerSteamID))
            {
                Globals_VoteSilent.VoteSilent_PlayerGaged.Add(TargetPlayerSteamID, true);
            }
            if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playersct = Helper.GetCounterTerroristController();
                playersct.ForEach(player => 
                {
                    Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
                    Globals_VoteSilent.VoteSilent_GetVoted[player.PlayerName] = 0;
                    Globals_VoteSilent.VoteSilent_countingCT = 0;
                    Globals_VoteSilent.VoteSilent_ShowMenuCT.Remove(player.SteamID);
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
        float percentage = Configs.GetConfigData().VoteSilent_Percentage;
        int requiredt = (int)Math.Ceiling(allTPlayers * (percentage / 100.0f));

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_ImmunityGroups) && Globals_VoteSilent.VoteSilent_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        
        if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.vote.same.player"],TargetPlayerName, Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName], requiredt);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementOnHalfVotes && Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredt/2)
        {
            var AllTPlayers = Helper.GetTerroristController();
            foreach (var players in AllTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(steamid))
                {
                    Globals_VoteSilent.VoteSilent_ShowMenuT.Add(steamid, true);
                }
            }
            
            Globals_VoteSilent.VoteSilent_timerT = Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementTimer;
            Globals_VoteSilent.VoteSilent_stopwatchT.Start();
            Globals_VoteSilent.VoteSilent_targetPlayerNameT = TargetPlayerName;
            Globals_VoteSilent.VoteSilent_countingT = Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName];
            Globals_VoteSilent.VoteSilent_requiredt = requiredt;
            Globals_VoteSilent.VoteSilent_targetPlayerIPT = TargerIP;
            Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
        {
            var playerst = Helper.GetTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votesilent.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votesilent.chat.message"], CallerName, TargetPlayerName, Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName], requiredt)
                );
            }
            
            if (Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementOnHalfVotes && Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredt/2 && !string.IsNullOrEmpty(Localizer!["votesilent.announce.halfvotes.chat.message"]))
            {
                playerst.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredt && !string.IsNullOrEmpty(Localizer!["votesilent.announce.silented.successfully.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.silented.successfully.message"], TargetPlayerName)
                );
            }
        }

        if (Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredt)
        {
            
            Json_VoteSilent.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteSilent_TimeInMins, Configs.GetConfigData().VoteSilent_TimeInMins, "Vote Silented", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Silented");
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
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Silented");
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
            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
            }
            GetTarget.VoiceFlags = VoiceFlags.Muted;
            if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(TargetPlayerSteamID))
            {
                Globals_VoteSilent.VoteSilent_PlayerGaged.Add(TargetPlayerSteamID, true);
            }
            if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playerst = Helper.GetTerroristController();
                playerst.ForEach(player => 
                {
                    Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
                    Globals_VoteSilent.VoteSilent_GetVoted[player.PlayerName] = 0;
                    Globals_VoteSilent.VoteSilent_countingT = 0;
                    Globals_VoteSilent.VoteSilent_ShowMenuT.Remove(player.SteamID);
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
        float percentage = Configs.GetConfigData().VoteSilent_Percentage;
        int requiredall = (int)Math.Ceiling(allPlayers * (percentage / 100.0f));

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteSilent_ImmunityGroups) && Globals_VoteSilent.VoteSilent_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        if (!Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            
            Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Add(TargetPlayerName);
            Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] + 1 : 1;
            
        }else if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votesilent.player.vote.same.player"],TargetPlayerName, Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementOnHalfVotes && Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteSilent.VoteSilent_ShowMenuBOTH.Add(steamid, true);
                }
            }
            
            Globals_VoteSilent.VoteSilent_timerBOTH = Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementTimer;
            Globals_VoteSilent.VoteSilent_stopwatchBOTH.Start();
            Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH = TargetPlayerName;
            Globals_VoteSilent.VoteSilent_countingBoth = Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName];
            Globals_VoteSilent.VoteSilent_requiredboth = requiredall;
            Globals_VoteSilent.VoteSilent_targetPlayerIPBOTH = TargerIP;
            Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH = TargetPlayerSteamID;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votesilent.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votesilent.chat.message"], CallerName, TargetPlayerName, Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementOnHalfVotes && Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votesilent.announce.halfvotes.chat.message"]))
        {
            var playerall = Helper.GetAllController();
            playerall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(steamid) || Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votesilent.announce.halfvotes.chat.message"]);
            });
        }
        
        if (Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votesilent.announce.silented.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votesilent.announce.silented.successfully.message"], TargetPlayerName);
        }
        

        if (Globals_VoteSilent.VoteSilent_GetVoted[TargetPlayerName] >= requiredall)
        {
            
            Json_VoteSilent.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteSilent_TimeInMins, Configs.GetConfigData().VoteSilent_TimeInMins, "Vote Silented", filename);

            if(Configs.GetConfigData().Log_SendLogToText)
            {
                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Silented");
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
            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_DiscordMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Silented");
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
            if (!string.IsNullOrEmpty(Localizer!["votesilent.player.silented.successfully.message"]))
            {
                Helper.AdvancedPrintToChat(GetTarget, Localizer["votesilent.player.silented.successfully.message"], Configs.GetConfigData().VoteSilent_TimeInMins);
            }
            GetTarget.VoiceFlags = VoiceFlags.Muted;
            if (!Globals_VoteSilent.VoteSilent_PlayerGaged.ContainsKey(TargetPlayerSteamID))
            {
                Globals_VoteSilent.VoteSilent_PlayerGaged.Add(TargetPlayerSteamID, true);
            }
            if (Globals_VoteSilent.VoteSilent_CallerVotedTo[Caller].Contains(TargetPlayerName))
            {
                var playersall = Helper.GetAllController();
                playersall.ForEach(player => 
                {
                    Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
                    Globals_VoteSilent.VoteSilent_GetVoted[player.PlayerName] = 0;
                    Globals_VoteSilent.VoteSilent_countingBoth = 0;
                    Globals_VoteSilent.VoteSilent_ShowMenuBOTH.Remove(player.SteamID);
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

        if (Globals_VoteSilent.VoteSilent_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteSilent.VoteSilent_CallerVotedTo[player])
            {
                if (Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteSilent.VoteSilent_GetVoted[votedPlayer]--;
                    Globals_VoteSilent.VoteSilent_countingCT = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteSilent.VoteSilent_countingCT - 1 : 1;
                    Globals_VoteSilent.VoteSilent_countingT = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteSilent.VoteSilent_countingT - 1 : 1;
                    Globals_VoteSilent.VoteSilent_countingBoth = Globals_VoteSilent.VoteSilent_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteSilent.VoteSilent_countingBoth - 1 : 1;
                }
            }
            Globals_VoteSilent.VoteSilent_CallerVotedTo.Remove(player);
        }

        if (Globals_VoteSilent.VoteSilent_Disable.ContainsKey(playerid))
        {
            Globals_VoteSilent.VoteSilent_Disable.Remove(playerid);
            foreach (var allplayers in Helper.GetAllController())
            {
                if(allplayers == null || !allplayers.IsValid)continue;
                var playerssteamid = allplayers.SteamID;
                if (!Globals_VoteSilent.VoteSilent_Disable.ContainsKey(playerssteamid))
                {
                    Globals_VoteSilent.VoteSilent_Disabled = false;
                }
            }
        }
        Globals_VoteSilent.VoteSilent_PlayerGaged.Remove(playerid);
        Globals_VoteSilent.VoteSilent_Immunity.Remove(playerid);
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        if (Configs.GetConfigData().VoteSilent_RemoveSilentedPlayersOnMapChange)
        {
            Json_VoteSilent.RemoveAnyByReason("Vote Silented", filename);
        }
        Helper.ClearVariablesVoteSilent();
    }
}