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

public class VoteKick
{
    private IStringLocalizer? Localizer;
    private string filename = "Kick.json";
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
        string Reason = "Kick Evasion";

        Json_VoteKick.PersonData personDataIP = Json_VoteKick.RetrievePersonDataByIp(PlayerIp, Configs.GetConfigData().VoteKick_TimeInMins, filename);
        Json_VoteKick.PersonData personDataID = Json_VoteKick.RetrievePersonDataById(playerid, Configs.GetConfigData().VoteKick_TimeInMins, filename);
        Json_VoteKick.PersonData personDataREASON = Json_VoteKick.RetrievePersonDataByReason(Reason, Configs.GetConfigData().VoteKick_TimeInMins, filename);
        DateTime personDate = DateTime.Now;

        if(Configs.GetConfigData().VoteKick_Mode == 2 && Json_VoteKick.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteKick_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteKick_EvasionPunishment && personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason)
            {
                Json_VoteKick.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteKick_EvasionPunishmentTimeInMins, "Kick Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Kick Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Kick Evasion");
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
            if(Configs.GetConfigData().VoteKick_DelayKick)
            {
                VoteGoldKingZ pluginInstance = new VoteGoldKingZ();
                Server.NextFrame(() =>
                {
                    pluginInstance.AddTimer(3.30f, () =>
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.delay.message"]))
                        {
                            Helper.AdvancedPrintToChat(player, Localizer!["votekick.player.delay.message"], Configs.GetConfigData().VoteKick_TimeInMins);
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
        
        if(Configs.GetConfigData().VoteKick_Mode == 3 && Json_VoteKick.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteKick_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteKick_EvasionPunishment && personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason)
            {
                Json_VoteKick.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteKick_EvasionPunishmentTimeInMins, "Kick Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Kick Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Kick Evasion");
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

        if(Configs.GetConfigData().VoteKick_Mode == 4 && (Json_VoteKick.IsPlayerIPRestricted(PlayerIp, Configs.GetConfigData().VoteKick_TimeInMins, filename) || Json_VoteKick.IsPlayerSteamIDRestricted(playerid, Configs.GetConfigData().VoteKick_TimeInMins, filename)))
        {
            if(Configs.GetConfigData().VoteKick_EvasionPunishment && (personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason || personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason))
            {
                Json_VoteKick.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, Configs.GetConfigData().VoteKick_EvasionPunishmentTimeInMins, "Kick Evasion", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Kick Evasion");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, playername, playerid.ToString(), PlayerIp!.ToString(), "Kick Evasion");
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
        if (string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) || @event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteKick_ImmunityGroups))
        {
            if (!Globals_VoteKick.VoteKick_Immunity.ContainsKey(playerid))
            {
                Globals_VoteKick.VoteKick_Immunity.Add(playerid, true);
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(Configs.GetConfigData().VoteKick_Mode == 0 || string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_CommandsToVote) || @event == null)return HookResult.Continue;
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
        string[] CenterMenuCommands = Configs.GetConfigData().VoteKick_CommandsToVote.Split(',');
        
        if (CenterMenuCommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            var VoteKickMenu = new ChatMenu(Localizer!["votekick.menu.name"]);            
            if(Configs.GetConfigData().VoteKick_TeamOnly)
            {
                if(CallerTeam == (byte)CsTeam.CounterTerrorist)
                {
                    var AllCTPlayers = Helper.GetCounterTerroristController();
                    var AllCTPlayersCount = Helper.GetCounterTerroristCount();
                    
                    if(AllCTPlayersCount < Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.minimum.needed"],Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllCTPlayersCount >= Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllCTPlayers)
                        {
                            if(Caller == players)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteKickMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuCT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
                if(CallerTeam == (byte)CsTeam.Terrorist)
                {
                    var AllTPlayers = Helper.GetTerroristController();
                    var AllTPlayersCount = Helper.GetTerroristCount();

                    if(AllTPlayersCount < Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.minimum.needed"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.minimum.needed"],Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers);
                        }
                        return HookResult.Continue;
                    }
                    if(AllTPlayersCount >= Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers)
                    {
                        foreach (var players in AllTPlayers)
                        {
                            if(Caller == players)continue;
                            var TargetPlayersNames = players.PlayerName;
                            var TargetPlayersUserID = (int)players.UserId!;
                            VoteKickMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuT(Caller, option, TargetPlayersUserID));
                        }
                    }
                }
            }else
            {
                var AllPlayers = Helper.GetAllController();
                var AllPlayersCount = Helper.GetAllCount();
                
                if(AllPlayersCount < Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers)
                {
                    if (!string.IsNullOrEmpty(Localizer!["votekick.minimum.needed"]))
                    {
                        Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.minimum.needed"],Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers);
                    }
                    return HookResult.Continue;
                }
                if(AllPlayersCount >= Configs.GetConfigData().VoteKick_StartOnMinimumOfXPlayers)
                {
                    foreach (var players in AllPlayers)
                    {
                        if(Caller == players)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteKickMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuALL(Caller, option, TargetPlayersUserID));
                    }
                }
            }
            VoteKickMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteKickMenu);
        }
        

        string[] Accept = Configs.GetConfigData().VoteKick_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteKick_TeamOnly)
            {
                if (Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteKick.VoteKick_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameCT)
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Add(Globals_VoteKick.VoteKick_targetPlayerNameCT);
                        Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameCT) ? Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT] + 1 : 1;
                        Globals_VoteKick.VoteKick_countingCT = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameCT) ? Globals_VoteKick.VoteKick_countingCT + 1 : 1;
                        
                    }else if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.yes"],Globals_VoteKick.VoteKick_targetPlayerNameCT, Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT], Globals_VoteKick.VoteKick_requiredct);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT] >= Globals_VoteKick.VoteKick_requiredct)
                    {
                        if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
                        {
                            Json_VoteKick.SaveToJsonFile(Globals_VoteKick.VoteKick_targetPlayerSTEAMCT, Globals_VoteKick.VoteKick_targetPlayerNameCT, Globals_VoteKick.VoteKick_targetPlayerIPCT!.ToString(), personDate, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);

                            if(Configs.GetConfigData().Log_SendLogToText)
                            {
                                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteKick.VoteKick_targetPlayerNameCT, Globals_VoteKick.VoteKick_targetPlayerSTEAMCT.ToString(), Globals_VoteKick.VoteKick_targetPlayerIPCT!.ToString(), "Vote Kicked");
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
                            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteKick.VoteKick_targetPlayerNameCT, Globals_VoteKick.VoteKick_targetPlayerSTEAMCT.ToString(), Globals_VoteKick.VoteKick_targetPlayerIPCT!.ToString(), "Vote Kicked");
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
                                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteKick.VoteKick_targetPlayerSTEAMCT.ToString(), Globals_VoteKick.VoteKick_targetPlayerNameCT);
                                    });
                                }
                            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                            {
                                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteKick.VoteKick_targetPlayerSTEAMCT.ToString(), Globals_VoteKick.VoteKick_targetPlayerNameCT);
                                    });
                                }
                            }
                        }
                        
                        Server.ExecuteCommand($"kick {Globals_VoteKick.VoteKick_targetPlayerNameCT}");

                        var playersct = Helper.GetCounterTerroristController();
                        if (!string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
                        {
                            playersct.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], Globals_VoteKick.VoteKick_targetPlayerNameCT)
                            );
                        }
                    }
                }
                if (Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteKick.VoteKick_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameT)
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Add(Globals_VoteKick.VoteKick_targetPlayerNameT);
                        Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameT) ? Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT] + 1 : 1;
                        Globals_VoteKick.VoteKick_countingT = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameT) ? Globals_VoteKick.VoteKick_countingT + 1 : 1;
                        
                    }else if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.yes"],Globals_VoteKick.VoteKick_targetPlayerNameT, Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT], Globals_VoteKick.VoteKick_requiredt);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT] >= Globals_VoteKick.VoteKick_requiredt)
                    {
                        if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
                        {
                            Json_VoteKick.SaveToJsonFile(Globals_VoteKick.VoteKick_targetPlayerSTEAMT, Globals_VoteKick.VoteKick_targetPlayerNameT, Globals_VoteKick.VoteKick_targetPlayerIPT!.ToString(), personDate, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);

                            if(Configs.GetConfigData().Log_SendLogToText)
                            {
                                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteKick.VoteKick_targetPlayerNameT, Globals_VoteKick.VoteKick_targetPlayerSTEAMT.ToString(), Globals_VoteKick.VoteKick_targetPlayerIPT!.ToString(), "Vote Kicked");
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
                            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteKick.VoteKick_targetPlayerNameT, Globals_VoteKick.VoteKick_targetPlayerSTEAMT.ToString(), Globals_VoteKick.VoteKick_targetPlayerIPT!.ToString(), "Vote Kicked");
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
                                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteKick.VoteKick_targetPlayerSTEAMT.ToString(), Globals_VoteKick.VoteKick_targetPlayerNameT);
                                    });
                                }
                            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                            {
                                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteKick.VoteKick_targetPlayerSTEAMT.ToString(), Globals_VoteKick.VoteKick_targetPlayerNameT);
                                    });
                                }
                            }
                        }
                        
                        Server.ExecuteCommand($"kick {Globals_VoteKick.VoteKick_targetPlayerNameT}");

                        var playerst = Helper.GetTerroristController();
                        if (!string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
                        {
                            playerst.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], Globals_VoteKick.VoteKick_targetPlayerNameT)
                            );
                        }
                    }
                }
            }else
            {
                if (Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteKick.VoteKick_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameBOTH)
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Add(Globals_VoteKick.VoteKick_targetPlayerNameBOTH);
                        Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) ? Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH] + 1 : 1;
                        Globals_VoteKick.VoteKick_countingBoth = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) ? Globals_VoteKick.VoteKick_countingBoth + 1 : 1;
                        
                    }else if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.yes"],Globals_VoteKick.VoteKick_targetPlayerNameBOTH, Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH], Globals_VoteKick.VoteKick_requiredboth);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH] >= Globals_VoteKick.VoteKick_requiredboth)
                    {
                        if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
                        {
                            Json_VoteKick.SaveToJsonFile(Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH, Globals_VoteKick.VoteKick_targetPlayerNameBOTH, Globals_VoteKick.VoteKick_targetPlayerIPBOTH!.ToString(), personDate, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);

                            if(Configs.GetConfigData().Log_SendLogToText)
                            {
                                var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteKick.VoteKick_targetPlayerNameBOTH, Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH.ToString(), Globals_VoteKick.VoteKick_targetPlayerIPBOTH!.ToString(), "Vote Kicked");
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
                            var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, Globals_VoteKick.VoteKick_targetPlayerNameBOTH, Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH.ToString(), Globals_VoteKick.VoteKick_targetPlayerIPBOTH!.ToString(), "Vote Kicked");
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
                                        _ = Helper.SendToDiscordWebhookNameLink(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH.ToString(), Globals_VoteKick.VoteKick_targetPlayerNameBOTH);
                                    });
                                }
                            }else if(Configs.GetConfigData().Log_SendLogToDiscordOnMode == 3)
                            {
                                if (!string.IsNullOrEmpty(Configs.GetConfigData().Log_DiscordMessageFormat))
                                {
                                    Server.NextFrame(() =>
                                    {
                                        _ = Helper.SendToDiscordWebhookNameLinkWithPicture(Configs.GetConfigData().Log_DiscordWebHookURL, replacerlogd, Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH.ToString(), Globals_VoteKick.VoteKick_targetPlayerNameBOTH);
                                    });
                                }
                            }
                        }
                        
                        Server.ExecuteCommand($"kick {Globals_VoteKick.VoteKick_targetPlayerNameBOTH}");

                        if (!string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["votekick.announce.kick.successfully.message"], Globals_VoteKick.VoteKick_targetPlayerNameBOTH);
                        }
                    }
                }
            }
        }
        string[] Refuse = Configs.GetConfigData().VoteKick_CommandsOnHalfVoteRefuse.Split(',');
        if (Refuse.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteKick_TeamOnly)
            {
                if (Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(CallerSteamID) && Globals_VoteKick.VoteKick_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameCT)
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Remove(Globals_VoteKick.VoteKick_targetPlayerNameCT);
                        Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameCT) ? Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT] - 1 : 1;
                        Globals_VoteKick.VoteKick_countingCT = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameCT) ? Globals_VoteKick.VoteKick_countingCT - 1 : 1;
                        
                    }else if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.no"],Globals_VoteKick.VoteKick_targetPlayerNameCT, Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameCT], Globals_VoteKick.VoteKick_requiredct);
                        }
                        return HookResult.Continue;
                    }
                    
                }
                if (Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(CallerSteamID) && Globals_VoteKick.VoteKick_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameT)
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Remove(Globals_VoteKick.VoteKick_targetPlayerNameT);
                        Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameT) ? Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT] - 1 : 1;
                        Globals_VoteKick.VoteKick_countingT = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameT) ? Globals_VoteKick.VoteKick_countingT - 1 : 1;
                        
                    }else if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.no"],Globals_VoteKick.VoteKick_targetPlayerNameT, Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameT], Globals_VoteKick.VoteKick_requiredt);
                        }
                        return HookResult.Continue;
                    }
                }
            }else
            {
                if (Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals_VoteKick.VoteKick_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameBOTH)
                    {
                        Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Remove(Globals_VoteKick.VoteKick_targetPlayerNameBOTH);
                        Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) ? Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH] - 1 : 1;
                        Globals_VoteKick.VoteKick_countingBoth = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) ? Globals_VoteKick.VoteKick_countingBoth - 1 : 1;
                        
                    }else if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(Globals_VoteKick.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals_VoteKick.VoteKick_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.no"],Globals_VoteKick.VoteKick_targetPlayerNameBOTH, Globals_VoteKick.VoteKick_GetVoted[Globals_VoteKick.VoteKick_targetPlayerNameBOTH], Globals_VoteKick.VoteKick_requiredboth);
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

        double xPercentage = Configs.GetConfigData().VoteKick_Percentage / 10.0;
        var CountCT = Helper.GetCounterTerroristCount();
        var requiredct = (int)Math.Ceiling(CountCT * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) && Globals_VoteKick.VoteKick_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        

        if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.vote.same.player"],TargetPlayerName, Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName], requiredct);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredct/2)
        {
            var AllCTPlayers = Helper.GetCounterTerroristController();
            foreach (var players in AllCTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(steamid))
                {
                    Globals_VoteKick.VoteKick_ShowMenuCT.Add(steamid, true);
                }
            }
            
            Globals_VoteKick.VoteKick_timerCT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
            Globals_VoteKick.VoteKick_stopwatchCT.Start();
            Globals_VoteKick.VoteKick_targetPlayerNameCT = TargetPlayerName;
            Globals_VoteKick.VoteKick_countingCT = Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName];
            Globals_VoteKick.VoteKick_requiredct = requiredct;
            Globals_VoteKick.VoteKick_targetPlayerIPCT = TargerIP;
            Globals_VoteKick.VoteKick_targetPlayerSTEAMCT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
        {
            var playersct = Helper.GetCounterTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votekick.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.chat.message"], CallerName, TargetPlayerName, Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName], requiredct)
                );
            }
            
            if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredct/2 && !string.IsNullOrEmpty(Localizer!["votekick.announce.halfvotes.chat.message"]))
            {
                playersct.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredct && !string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], TargetPlayerName)
                );
            }
        }

        if (Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredct)
        {
            if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
            {
                Json_VoteKick.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Kicked");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Kicked");
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

        double xPercentage = Configs.GetConfigData().VoteKick_Percentage / 10.0;
        var CountT = Helper.GetTerroristCount();
        var requiredt = (int)Math.Ceiling(CountT * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) && Globals_VoteKick.VoteKick_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }
        
        if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.vote.same.player"],TargetPlayerName, Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName], requiredt);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }


        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredt/2)
        {
            var AllTPlayers = Helper.GetTerroristController();
            foreach (var players in AllTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(steamid))
                {
                    Globals_VoteKick.VoteKick_ShowMenuT.Add(steamid, true);
                }
            }
            
            Globals_VoteKick.VoteKick_timerT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
            Globals_VoteKick.VoteKick_stopwatchT.Start();
            Globals_VoteKick.VoteKick_targetPlayerNameT = TargetPlayerName;
            Globals_VoteKick.VoteKick_countingT = Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName];
            Globals_VoteKick.VoteKick_requiredt = requiredt;
            Globals_VoteKick.VoteKick_targetPlayerIPT = TargerIP;
            Globals_VoteKick.VoteKick_targetPlayerSTEAMT = TargetPlayerSteamID;
        }
        

        if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
        {
            var playerst = Helper.GetTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votekick.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.chat.message"], CallerName, TargetPlayerName, Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName], requiredt)
                );
            }
            
            if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredt/2 && !string.IsNullOrEmpty(Localizer!["votekick.announce.halfvotes.chat.message"]))
            {
                playerst.ForEach(player => 
                {
                    var steamid = player.SteamID;
                    if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                    if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                    Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.halfvotes.chat.message"]);
                }); 
            }
            
            if (Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredt && !string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], TargetPlayerName)
                );
            }
        }

        if (Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredt)
        {
            if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
            {
                Json_VoteKick.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Kicked");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Kicked");
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

        double xPercentage = Configs.GetConfigData().VoteKick_Percentage / 10.0;
        var Countall = Helper.GetAllCount();
        var requiredall = (int)Math.Ceiling(Countall * xPercentage);

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) && Globals_VoteKick.VoteKick_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        if (!Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(Caller))
        {
            Globals_VoteKick.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            
            Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Add(TargetPlayerName);
            Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(TargetPlayerName) ? Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] + 1 : 1;
            
        }else if (Globals_VoteKick.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.vote.same.player"],TargetPlayerName, Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }


        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))continue;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))continue;
                if (!Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals_VoteKick.VoteKick_ShowMenuBOTH.Add(steamid, true);
                }
            }
            
            Globals_VoteKick.VoteKick_timerBOTH = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
            Globals_VoteKick.VoteKick_stopwatchBOTH.Start();
            Globals_VoteKick.VoteKick_targetPlayerNameBOTH = TargetPlayerName;
            Globals_VoteKick.VoteKick_countingBoth = Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName];
            Globals_VoteKick.VoteKick_requiredboth = requiredall;
            Globals_VoteKick.VoteKick_targetPlayerIPBOTH = TargerIP;
            Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH = TargetPlayerSteamID;
        }
        

        if (!string.IsNullOrEmpty(Localizer!["votekick.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votekick.chat.message"], CallerName, TargetPlayerName, Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votekick.announce.halfvotes.chat.message"]))
        {
            var playerall = Helper.GetCounterTerroristController();
            playerall.ForEach(player => 
            {
                var steamid = player.SteamID;
                if(Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(steamid) || Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(steamid))return;
                if(Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(steamid) || Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(steamid))return;
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.halfvotes.chat.message"]);
            });
        }
        
        if (Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votekick.announce.kick.successfully.message"], TargetPlayerName);
        }
        

        if (Globals_VoteKick.VoteKick_GetVoted[TargetPlayerName] >= requiredall)
        {
            if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
            {
                Json_VoteKick.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);

                if(Configs.GetConfigData().Log_SendLogToText)
                {
                    var replacerlog = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Kicked");
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
                var replacerlogd = Helper.ReplaceMessages(Configs.GetConfigData().Log_TextMessageFormat, Date, Time, TargetPlayerName, TargetPlayerSteamID.ToString(), TargerIP!.ToString(), "Vote Kicked");
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

        if (Globals_VoteKick.VoteKick_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals_VoteKick.VoteKick_CallerVotedTo[player])
            {
                if (Globals_VoteKick.VoteKick_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals_VoteKick.VoteKick_GetVoted[votedPlayer]--;
                    Globals_VoteKick.VoteKick_countingCT = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteKick.VoteKick_countingCT - 1 : 1;
                    Globals_VoteKick.VoteKick_countingT = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteKick.VoteKick_countingT - 1 : 1;
                    Globals_VoteKick.VoteKick_countingBoth = Globals_VoteKick.VoteKick_GetVoted.ContainsKey(votedPlayer) ? Globals_VoteKick.VoteKick_countingBoth - 1 : 1;
                }
            }
            Globals_VoteKick.VoteKick_CallerVotedTo.Remove(player);
        }
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        if (Configs.GetConfigData().VoteKick_AllowKickedPlayersToJoinOnMapChange)
        {
            Json_VoteKick.RemoveAnyByReason("Vote Kicked", filename);
        }
        Helper.ClearVariablesVoteKick();
    }
}