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
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)return;
        var playerid = player.SteamID;
        var playername = player.PlayerName;
        var GetPlayerIp = player.IpAddress;
        string[] parts = GetPlayerIp!.Split(':');
        string PlayerIp = parts[0];
        string Reason = "Kick Evasion";

        Helper.PersonData personDataIP = Helper.RetrievePersonDataByIp(PlayerIp, false, Configs.GetConfigData().VoteKick_TimeInMins, filename);
        Helper.PersonData personDataID = Helper.RetrievePersonDataById(playerid, false, Configs.GetConfigData().VoteKick_TimeInMins, filename);
        Helper.PersonData personDataREASON = Helper.RetrievePersonDataByReason(Reason, false, Configs.GetConfigData().VoteKick_TimeInMins, filename);
        DateTime personDate = DateTime.Now;

        if(Configs.GetConfigData().VoteKick_Mode == 2 && Helper.IsPlayerSteamIDRestricted(playerid, false, Configs.GetConfigData().VoteKick_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteKick_EvasionPunishment && personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason)
            {
                Helper.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_EvasionPunishmentTimeInMins, "Kick Evasion", filename);
            }
            Server.ExecuteCommand($"kick {playername}");
        }
        
        if(Configs.GetConfigData().VoteKick_Mode == 3 && Helper.IsPlayerIPRestricted(PlayerIp, false, Configs.GetConfigData().VoteKick_TimeInMins, filename))
        {
            if(Configs.GetConfigData().VoteKick_EvasionPunishment && personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason)
            {
                Helper.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_EvasionPunishmentTimeInMins, "Kick Evasion", filename);
            }
            Server.ExecuteCommand($"kick {playername}");
        }

        if(Configs.GetConfigData().VoteKick_Mode == 4 && (Helper.IsPlayerIPRestricted(PlayerIp, false, Configs.GetConfigData().VoteKick_TimeInMins, filename) || Helper.IsPlayerSteamIDRestricted(playerid, false, Configs.GetConfigData().VoteKick_TimeInMins, filename)))
        {
            if(Configs.GetConfigData().VoteKick_EvasionPunishment && (personDataIP != null && personDataIP.PlayerIPAddress == PlayerIp && personDataREASON != null && personDataID!.Reason != Reason || personDataID != null && personDataID.PlayerSteamID == playerid && personDataREASON != null && personDataID.Reason != Reason))
            {
                Helper.SaveToJsonFile(playerid, playername, PlayerIp!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_EvasionPunishmentTimeInMins, "Kick Evasion", filename);
            }
            Server.ExecuteCommand($"kick {playername}");
        }
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteKick_ImmunityGroups))
        {
            if (!Globals.VoteKick_Immunity.ContainsKey(playerid))
            {
                Globals.VoteKick_Immunity.Add(playerid, true);
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(Configs.GetConfigData().VoteKick_Mode == 0 || string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_CommandsToVote) || @event == null)return HookResult.Continue;
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
                    foreach (var players in AllCTPlayers)
                    {
                        if(Caller == players)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteKickMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuCT(Caller, option, TargetPlayersUserID));
                    }
                }
                if(CallerTeam == (byte)CsTeam.Terrorist)
                {
                    var AllTPlayers = Helper.GetTerroristController();
                    foreach (var players in AllTPlayers)
                    {
                        if(Caller == players)continue;
                        var TargetPlayersNames = players.PlayerName;
                        var TargetPlayersUserID = (int)players.UserId!;
                        VoteKickMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuT(Caller, option, TargetPlayersUserID));
                    }
                }
            }else
            {
                var AllPlayers = Helper.GetAllController();
                foreach (var players in AllPlayers)
                {
                    if(Caller == players)continue;
                    var TargetPlayersNames = players.PlayerName;
                    var TargetPlayersUserID = (int)players.UserId!;
                    VoteKickMenu.AddMenuOption(TargetPlayersNames, (Caller, option) => HandleMenuALL(Caller, option, TargetPlayersUserID));
                }
            }
            
            
            VoteKickMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteKickMenu);
        }


        
        //YES NO EVENT
        string[] Accept = Configs.GetConfigData().VoteKick_CommandsOnHalfVoteAccept.Split(',');

        if (Accept.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            DateTime personDate = DateTime.Now;
            if(Configs.GetConfigData().VoteKick_TeamOnly)
            {
                if (Globals.VoteKick_ShowMenuCT.ContainsKey(CallerSteamID) && Globals.VoteKick_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameCT)
                    {
                        Globals.VoteKick_CallerVotedTo[Caller].Add(Globals.VoteKick_targetPlayerNameCT);
                        Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT] = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameCT) ? Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT] + 1 : 1;
                        Globals.VoteKick_countingCT = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameCT) ? Globals.VoteKick_countingCT + 1 : 1;
                        
                    }else if (Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.yes"],Globals.VoteKick_targetPlayerNameCT, Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT], Globals.VoteKick_requiredct);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT] >= Globals.VoteKick_requiredct)
                    {
                        if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
                        {
                            Helper.SaveToJsonFile(Globals.VoteKick_targetPlayerSTEAMCT, Globals.VoteKick_targetPlayerNameCT, Globals.VoteKick_targetPlayerIPCT!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);
                        }
                        
                        Server.ExecuteCommand($"kick {Globals.VoteKick_targetPlayerNameCT}");

                        var playersct = Helper.GetCounterTerroristController();
                        if (!string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
                        {
                            playersct.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], Globals.VoteKick_targetPlayerNameCT)
                            );
                        }
                    }
                }
                if (Globals.VoteKick_ShowMenuT.ContainsKey(CallerSteamID) && Globals.VoteKick_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameT)
                    {
                        Globals.VoteKick_CallerVotedTo[Caller].Add(Globals.VoteKick_targetPlayerNameT);
                        Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT] = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameT) ? Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT] + 1 : 1;
                        Globals.VoteKick_countingT = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameT) ? Globals.VoteKick_countingT + 1 : 1;
                        
                    }else if (Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.yes"],Globals.VoteKick_targetPlayerNameT, Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT], Globals.VoteKick_requiredt);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT] >= Globals.VoteKick_requiredt)
                    {
                        if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
                        {
                            Helper.SaveToJsonFile(Globals.VoteKick_targetPlayerSTEAMT, Globals.VoteKick_targetPlayerNameT, Globals.VoteKick_targetPlayerIPT!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);
                        }
                        
                        Server.ExecuteCommand($"kick {Globals.VoteKick_targetPlayerNameT}");

                        var playerst = Helper.GetTerroristController();
                        if (!string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
                        {
                            playerst.ForEach(player => 
                            Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], Globals.VoteKick_targetPlayerNameT)
                            );
                        }
                    }
                }
            }else
            {
                if (Globals.VoteKick_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals.VoteKick_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameBOTH)
                    {
                        Globals.VoteKick_CallerVotedTo[Caller].Add(Globals.VoteKick_targetPlayerNameBOTH);
                        Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH] = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameBOTH) ? Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH] + 1 : 1;
                        Globals.VoteKick_countingBoth = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameBOTH) ? Globals.VoteKick_countingBoth + 1 : 1;
                        
                    }else if (Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.yes"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.yes"],Globals.VoteKick_targetPlayerNameBOTH, Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH], Globals.VoteKick_requiredboth);
                        }
                        return HookResult.Continue;
                    }

                    if (Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH] >= Globals.VoteKick_requiredboth)
                    {
                        if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
                        {
                            Helper.SaveToJsonFile(Globals.VoteKick_targetPlayerSTEAMBOTH, Globals.VoteKick_targetPlayerNameBOTH, Globals.VoteKick_targetPlayerIPBOTH!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);
                        }
                        
                        Server.ExecuteCommand($"kick {Globals.VoteKick_targetPlayerNameBOTH}");

                        if (!string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
                        {
                            Helper.AdvancedPrintToServer(Localizer["votekick.announce.kick.successfully.message"], Globals.VoteKick_targetPlayerNameBOTH);
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
                if (Globals.VoteKick_ShowMenuCT.ContainsKey(CallerSteamID) && Globals.VoteKick_ShowMenuCT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameCT)
                    {
                        Globals.VoteKick_CallerVotedTo[Caller].Remove(Globals.VoteKick_targetPlayerNameCT);
                        Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT] = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameCT) ? Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT] - 1 : 1;
                        Globals.VoteKick_countingCT = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameCT) ? Globals.VoteKick_countingCT - 1 : 1;
                        
                    }else if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameCT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameCT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.no"],Globals.VoteKick_targetPlayerNameCT, Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameCT], Globals.VoteKick_requiredct);
                        }
                        return HookResult.Continue;
                    }
                    
                }
                if (Globals.VoteKick_ShowMenuT.ContainsKey(CallerSteamID) && Globals.VoteKick_ShowMenuT[CallerSteamID] && Caller.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameT)
                    {
                        Globals.VoteKick_CallerVotedTo[Caller].Remove(Globals.VoteKick_targetPlayerNameT);
                        Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT] = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameT) ? Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT] - 1 : 1;
                        Globals.VoteKick_countingT = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameT) ? Globals.VoteKick_countingT - 1 : 1;
                        
                    }else if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameT) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameT)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.no"],Globals.VoteKick_targetPlayerNameT, Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameT], Globals.VoteKick_requiredt);
                        }
                        return HookResult.Continue;
                    }
                }
            }else
            {
                if (Globals.VoteKick_ShowMenuBOTH.ContainsKey(CallerSteamID) && Globals.VoteKick_ShowMenuBOTH[CallerSteamID])
                {
                    if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
                    {
                        Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
                    }

                    if (Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameBOTH)
                    {
                        Globals.VoteKick_CallerVotedTo[Caller].Remove(Globals.VoteKick_targetPlayerNameBOTH);
                        Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH] = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameBOTH) ? Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH] - 1 : 1;
                        Globals.VoteKick_countingBoth = Globals.VoteKick_GetVoted.ContainsKey(Globals.VoteKick_targetPlayerNameBOTH) ? Globals.VoteKick_countingBoth - 1 : 1;
                        
                    }else if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(Globals.VoteKick_targetPlayerNameBOTH) && Caller.PlayerName != Globals.VoteKick_targetPlayerNameBOTH)
                    {
                        if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.no"]))
                        {
                            Helper.AdvancedPrintToChat(Caller, Localizer!["votekick.player.vote.same.no"],Globals.VoteKick_targetPlayerNameBOTH, Globals.VoteKick_GetVoted[Globals.VoteKick_targetPlayerNameBOTH], Globals.VoteKick_requiredboth);
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

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) && Globals.VoteKick_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        

        //start counting
        if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
        {
            Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                Globals.VoteKick_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals.VoteKick_GetVoted[TargetPlayerName] = Globals.VoteKick_GetVoted.ContainsKey(TargetPlayerName) ? Globals.VoteKick_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.vote.same.player"],TargetPlayerName, Globals.VoteKick_GetVoted[TargetPlayerName], requiredct);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }

        //reached half
        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredct/2)
        {
            var AllCTPlayers = Helper.GetCounterTerroristController();
            foreach (var players in AllCTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if (!Globals.VoteKick_ShowMenuCT.ContainsKey(steamid))
                {
                    Globals.VoteKick_ShowMenuCT.Add(steamid, true);
                }
            }
            
            Globals.VoteKick_timerCT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
            Globals.VoteKick_stopwatchCT.Start();
            Globals.VoteKick_targetPlayerNameCT = TargetPlayerName;
            Globals.VoteKick_countingCT = Globals.VoteKick_GetVoted[TargetPlayerName];
            Globals.VoteKick_requiredct = requiredct;
            Globals.VoteKick_targetPlayerIPCT = TargerIP;
            Globals.VoteKick_targetPlayerSTEAMCT = TargetPlayerSteamID;
        }
        
        //messages
        if(CallerTeam == (byte)CsTeam.CounterTerrorist && TargetPlayerTeam == (byte)CsTeam.CounterTerrorist)
        {
            var playersct = Helper.GetCounterTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votekick.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.chat.message"], CallerName, TargetPlayerName, Globals.VoteKick_GetVoted[TargetPlayerName], requiredct)
                );
            }
            
            if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredct/2 && !string.IsNullOrEmpty(Localizer!["votekick.announce.halfvotes.chat.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.halfvotes.chat.message"])
                );
            }
            
            if (Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredct && !string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
            {
                playersct.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], TargetPlayerName)
                );
            }
        }
        //reached kicked
        if (Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredct)
        {
            if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
            {
                Helper.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);
            }
            
            Server.ExecuteCommand($"kick {TargetPlayerName}");
        }
        MenuManager.CloseActiveMenu(Caller);
    }
    private void HandleMenuT(CCSPlayerController Caller, ChatMenuOption option, int TargetPlayersUserID)
    {
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

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) && Globals.VoteKick_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        

        //start counting
        if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
        {
            Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                Globals.VoteKick_CallerVotedTo[Caller].Add(TargetPlayerName);
                Globals.VoteKick_GetVoted[TargetPlayerName] = Globals.VoteKick_GetVoted.ContainsKey(TargetPlayerName) ? Globals.VoteKick_GetVoted[TargetPlayerName] + 1 : 1;
            }
        }else if (Globals.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
            {
                if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.player"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.vote.same.player"],TargetPlayerName, Globals.VoteKick_GetVoted[TargetPlayerName], requiredt);
                }
                MenuManager.CloseActiveMenu(Caller);
                return;
            }
        }

        //reached half
        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredt/2)
        {
            var AllTPlayers = Helper.GetTerroristController();
            foreach (var players in AllTPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if (!Globals.VoteKick_ShowMenuT.ContainsKey(steamid))
                {
                    Globals.VoteKick_ShowMenuT.Add(steamid, true);
                }
            }
            
            Globals.VoteKick_timerT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
            Globals.VoteKick_stopwatchT.Start();
            Globals.VoteKick_targetPlayerNameT = TargetPlayerName;
            Globals.VoteKick_countingT = Globals.VoteKick_GetVoted[TargetPlayerName];
            Globals.VoteKick_requiredt = requiredt;
            Globals.VoteKick_targetPlayerIPT = TargerIP;
            Globals.VoteKick_targetPlayerSTEAMT = TargetPlayerSteamID;
        }
        
        //messages
        if(CallerTeam == (byte)CsTeam.Terrorist && TargetPlayerTeam == (byte)CsTeam.Terrorist)
        {
            var playerst = Helper.GetTerroristController();
            if (!string.IsNullOrEmpty(Localizer!["votekick.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.chat.message"], CallerName, TargetPlayerName, Globals.VoteKick_GetVoted[TargetPlayerName], requiredt)
                );
            }
            
            if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredt/2 && !string.IsNullOrEmpty(Localizer!["votekick.announce.halfvotes.chat.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.halfvotes.chat.message"])
                );
            }
            
            if (Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredt && !string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
            {
                playerst.ForEach(player => 
                Helper.AdvancedPrintToChat(player, Localizer["votekick.announce.kick.successfully.message"], TargetPlayerName)
                );
            }
        }
        //reached kicked
        if (Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredt)
        {
            if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
            {
                Helper.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);
            }
            
            Server.ExecuteCommand($"kick {TargetPlayerName}");
        }
        MenuManager.CloseActiveMenu(Caller);
    }

    private void HandleMenuALL(CCSPlayerController Caller, ChatMenuOption option, int TargetPlayersUserID)
    {
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

        if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteKick_ImmunityGroups) && Globals.VoteKick_Immunity.ContainsKey(TargetPlayerSteamID))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.is.immunity"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.is.immunity"], TargetPlayerName);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        

        //start counting
        if (!Globals.VoteKick_CallerVotedTo.ContainsKey(Caller))
        {
            Globals.VoteKick_CallerVotedTo[Caller] = new HashSet<string>();
        }

        if (!Globals.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            
            Globals.VoteKick_CallerVotedTo[Caller].Add(TargetPlayerName);
            Globals.VoteKick_GetVoted[TargetPlayerName] = Globals.VoteKick_GetVoted.ContainsKey(TargetPlayerName) ? Globals.VoteKick_GetVoted[TargetPlayerName] + 1 : 1;
            
        }else if (Globals.VoteKick_CallerVotedTo[Caller].Contains(TargetPlayerName))
        {
            if (!string.IsNullOrEmpty(Localizer!["votekick.player.vote.same.player"]))
            {
                Helper.AdvancedPrintToChat(Caller, Localizer["votekick.player.vote.same.player"],TargetPlayerName, Globals.VoteKick_GetVoted[TargetPlayerName], requiredall);
            }
            MenuManager.CloseActiveMenu(Caller);
            return;
        }

        //reached half
        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredall/2)
        {
            var AllPlayers = Helper.GetAllController();
            foreach (var players in AllPlayers)
            {
                if(players == null || !players.IsValid)continue;
                var steamid = players.SteamID;
                if (!Globals.VoteKick_ShowMenuBOTH.ContainsKey(steamid))
                {
                    Globals.VoteKick_ShowMenuBOTH.Add(steamid, true);
                }
            }
            
            Globals.VoteKick_timerBOTH = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
            Globals.VoteKick_stopwatchBOTH.Start();
            Globals.VoteKick_targetPlayerNameBOTH = TargetPlayerName;
            Globals.VoteKick_countingBoth = Globals.VoteKick_GetVoted[TargetPlayerName];
            Globals.VoteKick_requiredboth = requiredall;
            Globals.VoteKick_targetPlayerIPBOTH = TargerIP;
            Globals.VoteKick_targetPlayerSTEAMBOTH = TargetPlayerSteamID;
        }
        
        //messages
        if (!string.IsNullOrEmpty(Localizer!["votekick.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votekick.chat.message"], CallerName, TargetPlayerName, Globals.VoteKick_GetVoted[TargetPlayerName], requiredall);
        }
        
        if (Configs.GetConfigData().VoteKick_CenterMessageAnnouncementOnHalfVotes && Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredall/2 && !string.IsNullOrEmpty(Localizer!["votekick.announce.halfvotes.chat.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votekick.announce.halfvotes.chat.message"]);
        }
        
        if (Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredall && !string.IsNullOrEmpty(Localizer!["votekick.announce.kick.successfully.message"]))
        {
            Helper.AdvancedPrintToServer(Localizer["votekick.announce.kick.successfully.message"], TargetPlayerName);
        }
        
        //reached kicked
        if (Globals.VoteKick_GetVoted[TargetPlayerName] >= requiredall)
        {
            if (Configs.GetConfigData().VoteKick_Mode == 2 || Configs.GetConfigData().VoteKick_Mode == 3 || Configs.GetConfigData().VoteKick_Mode == 4)
            {
                Helper.SaveToJsonFile(TargetPlayerSteamID, TargetPlayerName, TargerIP!.ToString(), personDate, false, Configs.GetConfigData().VoteKick_TimeInMins, "Vote Kicked", filename);
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

        if (Globals.VoteKick_CallerVotedTo.ContainsKey(player))
        {
            foreach (var votedPlayer in Globals.VoteKick_CallerVotedTo[player])
            {
                if (Globals.VoteKick_GetVoted.ContainsKey(votedPlayer))
                {
                    Globals.VoteKick_GetVoted[votedPlayer]--;
                    Globals.VoteKick_countingCT = Globals.VoteKick_GetVoted.ContainsKey(votedPlayer) ? Globals.VoteKick_countingCT - 1 : 1;
                    Globals.VoteKick_countingT = Globals.VoteKick_GetVoted.ContainsKey(votedPlayer) ? Globals.VoteKick_countingT - 1 : 1;
                    Globals.VoteKick_countingBoth = Globals.VoteKick_GetVoted.ContainsKey(votedPlayer) ? Globals.VoteKick_countingBoth - 1 : 1;
                }
            }
            Globals.VoteKick_CallerVotedTo.Remove(player);
        }
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        if (Configs.GetConfigData().VoteKick_AllowKickedPlayersToJoinOnMapChange)
        {
            Helper.RemoveAnyByReason("Vote Kicked", filename);
        }
        Helper.ClearVariablesVoteKick();
    }
}