using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Menu;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
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
using CounterStrikeSharp.API.Core.Attributes;

namespace Vote_GoldKingZ;

public class VoteBanned
{
    private IStringLocalizer? Localizer;
    private string filename = "Banned.json";
    public void SetStringLocalizer(IStringLocalizer stringLocalizer)
    {
        Localizer = stringLocalizer;
    }
    
    public void OnClientPutInServer(int playerSlot)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";

        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return;
        var playerid = player.SteamID;
        var playername = player.PlayerName;
        var GetPlayerIp = player.IpAddress;
        string[] parts = GetPlayerIp!.Split(':');
        string PlayerIp = parts[0];
        string Reason = "Banned Evasion";

        Json_VoteBanned.PersonData personDataIP = Json_VoteBanned.RetrievePersonDataByIp(PlayerIp, Configs.GetConfigData().VoteBanned_TimeInDays, filename);
        Json_VoteBanned.PersonData personDataID = Json_VoteBanned.RetrievePersonDataById(playerid, Configs.GetConfigData().VoteBanned_TimeInDays, filename);
        Json_VoteBanned.PersonData personDataREASON = Json_VoteBanned.RetrievePersonDataByReason(Reason, Configs.GetConfigData().VoteBanned_TimeInDays, filename);
        DateTime personDate = DateTime.Now;

        if(Configs.GetConfigData().VoteBanned_Mode == 1 && Json_VoteBanned.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteBanned_TimeInDays, filename))
        {
            if(Configs.GetConfigData().VoteBanned_EvasionPunishment && personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason)
            {
                Json_VoteBanned.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteBanned_EvasionPunishmentTimeInDays, "Banned Evasion", filename);
                
                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Banned Evasion");
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

                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Banned Evasion");
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
            
            if(Configs.GetConfigData().VoteBanned_DelayKick)
            {
                VoteGoldKingZ pluginInstance = new VoteGoldKingZ();
                Server.NextFrame(() =>
                {
                    pluginInstance.AddTimer(3.30f, () =>
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.delay.message"]))
                        {
                            Helper.AdvancedPrintToChat(player, Localizer!["votebanned.player.delay.message"], Configs.GetConfigData().VoteBanned_TimeInDays);
                        }
                    });
                    
                    pluginInstance.AddTimer(10.30f, () =>
                    {
                        Server.ExecuteCommand($"kick {playername}");
                        if (pluginInstance is IDisposable disposableInstance)
                        {
                            disposableInstance.Dispose();
                        }
                    });
                });
            }else
            {
                Server.ExecuteCommand($"kick {playername}");
            }
        }
        
        if(Configs.GetConfigData().VoteBanned_Mode == 2 && Json_VoteBanned.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteBanned_TimeInDays, filename))
        {
            if(Configs.GetConfigData().VoteBanned_EvasionPunishment && personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason)
            {
                Json_VoteBanned.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteBanned_EvasionPunishmentTimeInDays, "Banned Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Banned Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Banned Evasion");
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
            Server.ExecuteCommand($"kick {playername}");
        }

        if(Configs.GetConfigData().VoteBanned_Mode == 3 && (Json_VoteBanned.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteBanned_TimeInDays, filename) || Json_VoteBanned.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteBanned_TimeInDays, filename)))
        {
            if(Configs.GetConfigData().VoteBanned_EvasionPunishment && (personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason || personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason))
            {
                Json_VoteBanned.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteBanned_EvasionPunishmentTimeInDays, "Banned Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Banned Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Banned Evasion");
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
            Server.ExecuteCommand($"kick {playername}");
        }
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (string.IsNullOrEmpty(Configs.GetConfigData().VoteBanned_ImmunityGroups) || @event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteBanned_ImmunityGroups))
        {
            if (!Globals_VoteBanned.VoteBanned_Immunity.ContainsKey(playerid))
            {
                Globals_VoteBanned.VoteBanned_Immunity.Add(playerid, true);
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(Configs.GetConfigData().VoteBanned_Mode == 0 || string.IsNullOrEmpty(Configs.GetConfigData().VoteBanned_CommandsToVote) || @event == null)return HookResult.Continue;

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
        string[] CenterMenuCommands = Configs.GetConfigData().VoteBanned_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteBannedMenu = new ChatMenu(Localizer!["votebanned.menu.name"]);            
            if(Configs.GetConfigData().VoteBanned_TeamOnly)
            {
                if(CallerTeam == (byte)CsTeam.CounterTerrorist)
                {
                    var AllCTPlayers = Helper.GetCounterTerroristController();
                    var AllCTPlayersCount = Helper.GetCounterTerroristCount();
                    
                    if(AllCTPlayersCount < Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.minimum.needed"],Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount >= Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllCTPlayers)
                        {
                            if(Caller == players)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteBannedMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuCT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
                if(CallerTeam == (byte)CsTeam.Terrorist)
                {
                    var AllTPlayers = Helper.GetTerroristController();
                    var AllTPlayersCount = Helper.GetTerroristCount();

                    if(AllTPlayersCount < Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.minimum.needed"],Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount >= Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllTPlayers)
                        {
                            if(Caller == players)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteBannedMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
            }else
            {
                var AllPlayers = Helper.GetAllController();
                var AllPlayersCount = Helper.GetAllCount();
                
                if(AllPlayersCount < Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votebanned.minimum.needed"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.minimum.needed"],Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount >= Configs.GetConfigData().VoteBanned_StartOnMinimumOfXPlayers)
                {
                    foreach (var players in AllPlayers)
                    {
                        if(Caller == players)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteBannedMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuALL(Caller, option, TargetPlayersUserID));
                    }
                }
            }
            
            
            VoteBannedMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteBannedMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteBanned_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteBanned_TeamOnly)
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteBanned.VoteBanned_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameCT)
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Add(Globals_VoteBanned.VoteBanned_targetPlayerNameCT);
                        Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) ? Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT] + 1 : 1;
                        Globals_VoteBanned.VoteBanned_countingCT = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) ? Globals_VoteBanned.VoteBanned_countingCT + 1 : 1;
                        
                    }else if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.player.vote.same.yes"],Globals_VoteBanned.VoteBanned_targetPlayerNameCT, Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT], Globals_VoteBanned.VoteBanned_requiredct);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT] >= Globals_VoteBanned.VoteBanned_requiredct)
                    {
                        if (Configs.GetConfigData().VoteBanned_Mode == 1 || Configs.GetConfigData().VoteBanned_Mode == 2 || Configs.GetConfigData().VoteBanned_Mode == 3)
                        {
                            Json_VoteBanned.SaveToJsonFile(Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT, Globals_VoteBanned.VoteBanned_targetPlayerNameCT, Globals_VoteBanned.VoteBanned_targetPlayerIPCT!.ToString(), personDate, Configs.GetConfigData().VoteBanned_TimeInDays, "Vote Banned", filename);

                            if(Configs.GetConfigData().Log_SendLogToText)
                            {
                                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteBanned.VoteBanned_targetPlayerNameCT, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerIPCT!.ToString(), "Vote Banned");
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

                            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteBanned.VoteBanned_targetPlayerNameCT, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerIPCT!.ToString(), "Vote Banned");
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
                                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerNameCT);
                                    });
                                }
                            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                            {
                                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerNameCT);
                                    });
                                }
                            }
                        }
                        
                        Server.ExecuteCommand($"kick {Globals_VoteBanned.VoteBanned_targetPlayerNameCT}");

                        var playersct = Helper.GetCounterTerroristController();
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.announce.banned.successfully.message"]))
                        {
                            playersct.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.banned.successfully.message"], Globals_VoteBanned.VoteBanned_targetPlayerNameCT)
                            );
                        }
                    }
                }
                if (Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteBanned.VoteBanned_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameT)
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Add(Globals_VoteBanned.VoteBanned_targetPlayerNameT);
                        Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameT) ? Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT] + 1 : 1;
                        Globals_VoteBanned.VoteBanned_countingT = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameT) ? Globals_VoteBanned.VoteBanned_countingT + 1 : 1;
                        
                    }else if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.player.vote.same.yes"],Globals_VoteBanned.VoteBanned_targetPlayerNameT, Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT], Globals_VoteBanned.VoteBanned_requiredt);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT] >= Globals_VoteBanned.VoteBanned_requiredt)
                    {
                        if (Configs.GetConfigData().VoteBanned_Mode == 1 || Configs.GetConfigData().VoteBanned_Mode == 2 || Configs.GetConfigData().VoteBanned_Mode == 3)
                        {
                            Json_VoteBanned.SaveToJsonFile(Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT, Globals_VoteBanned.VoteBanned_targetPlayerNameT, Globals_VoteBanned.VoteBanned_targetPlayerIPT!.ToString(), personDate, Configs.GetConfigData().VoteBanned_TimeInDays, "Vote Banned", filename);

                            if(Configs.GetConfigData().Log_SendLogToText)
                            {
                                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteBanned.VoteBanned_targetPlayerNameT, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerIPT!.ToString(), "Vote Banned");
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
                            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteBanned.VoteBanned_targetPlayerNameT, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerIPT!.ToString(), "Vote Banned");
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
                                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerNameT);
                                    });
                                }
                            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                            {
                                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerNameT);
                                    });
                                }
                            }
                        }
                        
                        Server.ExecuteCommand($"kick {Globals_VoteBanned.VoteBanned_targetPlayerNameT}");

                        var playerst = Helper.GetTerroristController();
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.announce.banned.successfully.message"]))
                        {
                            playerst.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.banned.successfully.message"], Globals_VoteBanned.VoteBanned_targetPlayerNameT)
                            );
                        }
                    }
                }
            }else
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteBanned.VoteBanned_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH)
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Add(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH);
                        Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) ? Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH] + 1 : 1;
                        Globals_VoteBanned.VoteBanned_countingBoth = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) ? Globals_VoteBanned.VoteBanned_countingBoth + 1 : 1;
                        
                    }else if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.player.vote.same.yes"],Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH, Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH], Globals_VoteBanned.VoteBanned_requiredboth);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH] >= Globals_VoteBanned.VoteBanned_requiredboth)
                    {
                        if (Configs.GetConfigData().VoteBanned_Mode == 1 || Configs.GetConfigData().VoteBanned_Mode == 2 || Configs.GetConfigData().VoteBanned_Mode == 3)
                        {
                            Json_VoteBanned.SaveToJsonFile(Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH, Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH, Globals_VoteBanned.VoteBanned_targetPlayerIPBOTH!.ToString(), personDate, Configs.GetConfigData().VoteBanned_TimeInDays, "Vote Banned", filename);

                            if(Configs.GetConfigData().Log_SendLogToText)
                            {
                                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerIPBOTH!.ToString(), "Vote Banned");
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

                            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerIPBOTH!.ToString(), "Vote Banned");
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
                                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH);
                                    });
                                }
                            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                            {
                                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH.ToString(), Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH);
                                    });
                                }
                            }
                        }
                        

                        Server.ExecuteCommand($"kick {Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH}");

                        if (!string.IsNullOrEmpty(Localizer!["votebanned.announce.banned.successfully.message"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["votebanned.announce.banned.successfully.message"], Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH);
                        }
                    }
                }
            }
        }
        string[] Refuse = Configs.GetConfigData().VoteBanned_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteBanned_TeamOnly)
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteBanned.VoteBanned_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameCT)
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Remove(Globals_VoteBanned.VoteBanned_targetPlayerNameCT);
                        Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) ? Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT] - 1 : 1;
                        Globals_VoteBanned.VoteBanned_countingCT = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) ? Globals_VoteBanned.VoteBanned_countingCT - 1 : 1;
                        
                    }else if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.player.vote.same.no"],Globals_VoteBanned.VoteBanned_targetPlayerNameCT, Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameCT], Globals_VoteBanned.VoteBanned_requiredct);
                        }
                        return HookResult.Continue;
                    }
                    
                }
                if (Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteBanned.VoteBanned_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameT)
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Remove(Globals_VoteBanned.VoteBanned_targetPlayerNameT);
                        Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameT) ? Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT] - 1 : 1;
                        Globals_VoteBanned.VoteBanned_countingT = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameT) ? Globals_VoteBanned.VoteBanned_countingT - 1 : 1;
                        
                    }else if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameT) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.player.vote.same.no"],Globals_VoteBanned.VoteBanned_targetPlayerNameT, Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameT], Globals_VoteBanned.VoteBanned_requiredt);
                        }
                        return HookResult.Continue;
                    }
                }
            }else
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteBanned.VoteBanned_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH)
                    {
                        Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Remove(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH);
                        Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) ? Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH] - 1 : 1;
                        Globals_VoteBanned.VoteBanned_countingBoth = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) ? Globals_VoteBanned.VoteBanned_countingBoth - 1 : 1;
                        
                    }else if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votebanned.player.vote.same.no"],Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH, Globals_VoteBanned.VoteBanned_GetVoted[Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH], Globals_VoteBanned.VoteBanned_requiredboth);
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

        double xPercentage = Configs.GetConfigData().VoteBanned_Percentage / 10.0;
        var CountCT = Helper.GetCounterTerroristCount();
        var requiredct = (int)Math.Ceiling(CountCT * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteBanned_ImmunityGroups) && Globals_VoteBanned.VoteBanned_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votebanned.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votebanned.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votebanned.player.vote.same.player"],TargetPlayerName, Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName], requiredct);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }

        if (Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementOnHalfVotes && Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredct/2)
        {
            var AllCTPlayers = Helper.GetCounterTerroristController();
            foreach (var players in AllCTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid))
                {
                    Globals_VoteBanned.VoteBanned_ShowMenuCT.Add(steamid, true);
                }
            }
            
            Globals_VoteBanned.VoteBanned_timerCT = Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementTimer;
            Globals_VoteBanned.VoteBanned_stopwatchCT.Start();
            Globals_VoteBanned.VoteBanned_targetPlayerNameCT = TargetPlayerName;
            Globals_VoteBanned.VoteBanned_countingCT = Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName];
            Globals_VoteBanned.VoteBanned_requiredct = requiredct;
            Globals_VoteBanned.VoteBanned_targetPlayerIPCT = TargerIP;
            Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT = TargetPlayerSteamID;
        }
        
        if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
        {
            var playersct = Helper.GetCounterTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votebanned.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votebanned.chat.message"], CallerName, TargetPlayerName, Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName], requiredct)
                );
            }
            
            if (Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementOnHalfVotes && Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredct/2 && !string.IsNullOrEmpty(Localizer!["votebanned.announce.halfvotes.chat.message"]))
            {
                playersct.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredct && !string.IsNullOrEmpty(Localizer!["votebanned.announce.banned.successfully.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.banned.successfully.message"], TargetPlayerName)
                );
            }
        }
        
        if (Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredct)
        {
            if (Configs.GetConfigData().VoteBanned_Mode == 1 || Configs.GetConfigData().VoteBanned_Mode == 2 || Configs.GetConfigData().VoteBanned_Mode == 3)
            {
                Json_VoteBanned.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteBanned_TimeInDays, "Vote Banned", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Banned");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Banned");
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
            }
            
            Server.ExecuteCommand($"kick {TargetPlayerName}");
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

        double xPercentage = Configs.GetConfigData().VoteBanned_Percentage / 10.0;
        var CountT = Helper.GetTerroristCount();
        var requiredt = (int)Math.Ceiling(CountT * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteBanned_ImmunityGroups) && Globals_VoteBanned.VoteBanned_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votebanned.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votebanned.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        
        if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votebanned.player.vote.same.player"],TargetPlayerName, Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName], requiredt);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementOnHalfVotes && Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredt/2)
        {
            var AllTPlayers = Helper.GetTerroristController();
            foreach (var players in AllTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid))
                {
                    Globals_VoteBanned.VoteBanned_ShowMenuT.Add(steamid, true);
                }
            }
            
            Globals_VoteBanned.VoteBanned_timerT = Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementTimer;
            Globals_VoteBanned.VoteBanned_stopwatchT.Start();
            Globals_VoteBanned.VoteBanned_targetPlayerNameT = TargetPlayerName;
            Globals_VoteBanned.VoteBanned_countingT = Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName];
            Globals_VoteBanned.VoteBanned_requiredt = requiredt;
            Globals_VoteBanned.VoteBanned_targetPlayerIPT = TargerIP;
            Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
        {
            var playerst = Helper.GetTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votebanned.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votebanned.chat.message"], CallerName, TargetPlayerName, Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName], requiredt)
                );
            }
            
            if (Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementOnHalfVotes && Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredt/2 && !string.IsNullOrEmpty(Localizer!["votebanned.announce.halfvotes.chat.message"]))
            {
                playerst.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredt && !string.IsNullOrEmpty(Localizer!["votebanned.announce.banned.successfully.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.banned.successfully.message"], TargetPlayerName)
                );
            }
        }

        if (Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredt)
        {
            if (Configs.GetConfigData().VoteBanned_Mode == 1 || Configs.GetConfigData().VoteBanned_Mode == 2 || Configs.GetConfigData().VoteBanned_Mode == 3)
            {
                Json_VoteBanned.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteBanned_TimeInDays, "Vote Banned", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Banned");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Banned");
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
            }
            
            
            Server.ExecuteCommand($"kick {TargetPlayerName}");
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

        double xPercentage = Configs.GetConfigData().VoteBanned_Percentage / 10.0;
        var Countall = Helper.GetAllCount();
        var requiredall = (int)Math.Ceiling(Countall * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteBanned_ImmunityGroups) && Globals_VoteBanned.VoteBanned_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votebanned.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votebanned.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        if (!Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            
            Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Add(TargetPlayerName);
            Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] + 1 : 1;
            
        }else if (Globals_VoteBanned.VoteBanned_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votebanned.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votebanned.player.vote.same.player"],TargetPlayerName, Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementOnHalfVotes && Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteBanned.VoteBanned_ShowMenuBOTH.Add(steamid, true);
                }
            }
            
            Globals_VoteBanned.VoteBanned_timerBOTH = Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementTimer;
            Globals_VoteBanned.VoteBanned_stopwatchBOTH.Start();
            Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH = TargetPlayerName;
            Globals_VoteBanned.VoteBanned_countingBoth = Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName];
            Globals_VoteBanned.VoteBanned_requiredboth = requiredall;
            Globals_VoteBanned.VoteBanned_targetPlayerIPBOTH = TargerIP;
            Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH = TargetPlayerSteamID;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votebanned.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votebanned.chat.message"], CallerName, TargetPlayerName, Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementOnHalfVotes && Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votebanned.announce.halfvotes.chat.message"]))
        {
            var playersall = Helper.GetCounterTerroristController();
            playersall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid) || Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votebanned.announce.halfvotes.chat.message"]);
            }); 
        }
        
        if (Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votebanned.announce.banned.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votebanned.announce.banned.successfully.message"], TargetPlayerName);
        }
        

        if (Globals_VoteBanned.VoteBanned_GetVoted[TargetPlayerName] >= requiredall)
        {
            if (Configs.GetConfigData().VoteBanned_Mode == 1 || Configs.GetConfigData().VoteBanned_Mode == 2 || Configs.GetConfigData().VoteBanned_Mode == 3)
            {
                Json_VoteBanned.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteBanned_TimeInDays, "Vote Banned", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Banned");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Banned");
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
            }
            
            Server.ExecuteCommand($"kick {TargetPlayerName}");
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

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;

        if (Globals_VoteBanned.VoteBanned_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteBanned.VoteBanned_CallerVotedTo[player])
            {
                if (Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteBanned.VoteBanned_GetVoted[votedPlayer]--;
                    Globals_VoteBanned.VoteBanned_countingCT = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteBanned.VoteBanned_countingCT - 1 : 1;
                    Globals_VoteBanned.VoteBanned_countingT = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteBanned.VoteBanned_countingT - 1 : 1;
                    Globals_VoteBanned.VoteBanned_countingBoth = Globals_VoteBanned.VoteBanned_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteBanned.VoteBanned_countingBoth - 1 : 1;
                }
            }
            Globals_VoteBanned.VoteBanned_CallerVotedTo.Remove(player);
        }
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariablesVoteBan();
    }
}