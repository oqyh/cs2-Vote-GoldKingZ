using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using Vote_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Entities;
using System.Text.RegularExpressions;

namespace Vote_GoldKingZ;

public class Helper
{


    public static void AdvancedPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPrintToServer(string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                    return true;
            }
            else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                    return true;
            }
        }
        return false;
    }
    public static List<CCSPlayerController> GetCounterTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && p.Team == CsTeam.CounterTerrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && p.Team == CsTeam.Terrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid).ToList();
        return playerList;
    }
    public static int GetCounterTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p.TeamNum == (byte)CsTeam.CounterTerrorist && !p.IsHLTV);
    }
    public static int GetTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p.TeamNum == (byte)CsTeam.Terrorist && !p.IsHLTV);
    }
    public static int GetAllCount()
    {
        return Utilities.GetPlayers().Count(p => !p.IsHLTV);
    }
    
    public static void ClearVariablesVoteKick()
    {
        Globals_VoteKick.VoteKick_timerCT = 0f;
        Globals_VoteKick.VoteKick_timerT = 0f;
        Globals_VoteKick.VoteKick_timerBOTH = 0f;
        Globals_VoteKick.VoteKick_targetPlayerNameCT = "";
        Globals_VoteKick.VoteKick_targetPlayerNameT = "";
        Globals_VoteKick.VoteKick_targetPlayerNameBOTH = "";
        Globals_VoteKick.VoteKick_targetPlayerIPCT = "";
        Globals_VoteKick.VoteKick_targetPlayerIPT = "";
        Globals_VoteKick.VoteKick_targetPlayerIPBOTH = "";
        Globals_VoteKick.VoteKick_targetPlayerSTEAMCT = 0;
        Globals_VoteKick.VoteKick_targetPlayerSTEAMT = 0;
        Globals_VoteKick.VoteKick_targetPlayerSTEAMBOTH = 0;
        Globals_VoteKick.VoteKick_ReachHalfVoteCT = false;
        Globals_VoteKick.VoteKick_ReachHalfVoteT = false;
        Globals_VoteKick.VoteKick_ReachHalfVoteBoth = false;
        Globals_VoteKick.VoteKick_countingCT = 0;
        Globals_VoteKick.VoteKick_countingT = 0;
        Globals_VoteKick.VoteKick_countingBoth = 0;
        Globals_VoteKick.VoteKick_requiredct = 0;
        Globals_VoteKick.VoteKick_requiredt = 0;
        Globals_VoteKick.VoteKick_requiredboth = 0;
        Globals_VoteKick.VoteKick_ShowMenuCT.Clear();
        Globals_VoteKick.VoteKick_ShowMenuT.Clear();
        Globals_VoteKick.VoteKick_ShowMenuBOTH.Clear();
        Globals_VoteKick.VoteKick_Immunity.Clear();
        Globals_VoteKick.VoteKick_GetVoted.Clear();
        Globals_VoteKick.VoteKick_CallerVotedTo.Clear();
    }
    public static void ClearVariablesVoteBanned()
    {
        Globals_VoteBanned.VoteBanned_timerCT = 0f;
        Globals_VoteBanned.VoteBanned_timerT = 0f;
        Globals_VoteBanned.VoteBanned_timerBOTH = 0f;
        Globals_VoteBanned.VoteBanned_targetPlayerNameCT = "";
        Globals_VoteBanned.VoteBanned_targetPlayerNameT = "";
        Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH = "";
        Globals_VoteBanned.VoteBanned_targetPlayerIPCT = "";
        Globals_VoteBanned.VoteBanned_targetPlayerIPT = "";
        Globals_VoteBanned.VoteBanned_targetPlayerIPBOTH = "";
        Globals_VoteBanned.VoteBanned_targetPlayerSTEAMCT = 0;
        Globals_VoteBanned.VoteBanned_targetPlayerSTEAMT = 0;
        Globals_VoteBanned.VoteBanned_targetPlayerSTEAMBOTH = 0;
        Globals_VoteBanned.VoteBanned_ReachHalfVoteCT = false;
        Globals_VoteBanned.VoteBanned_ReachHalfVoteT = false;
        Globals_VoteBanned.VoteBanned_ReachHalfVoteBoth = false;
        Globals_VoteBanned.VoteBanned_countingCT = 0;
        Globals_VoteBanned.VoteBanned_countingT = 0;
        Globals_VoteBanned.VoteBanned_countingBoth = 0;
        Globals_VoteBanned.VoteBanned_requiredct = 0;
        Globals_VoteBanned.VoteBanned_requiredt = 0;
        Globals_VoteBanned.VoteBanned_requiredboth = 0;
        Globals_VoteBanned.VoteBanned_ShowMenuCT.Clear();
        Globals_VoteBanned.VoteBanned_ShowMenuT.Clear();
        Globals_VoteBanned.VoteBanned_ShowMenuBOTH.Clear();
        Globals_VoteBanned.VoteBanned_Immunity.Clear();
        Globals_VoteBanned.VoteBanned_GetVoted.Clear();
        Globals_VoteBanned.VoteBanned_CallerVotedTo.Clear();
    }
    public static bool IsDirectoryReadable(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if ((directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                return false;
            }

            string[] files = Directory.GetFiles(directoryPath);

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }
    public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                if (!directoryInfo.Exists)
                    throw new DirectoryNotFoundException($"Directory '{dirPath}' not found.");

                string tempFilePath = Path.Combine(dirPath, Path.GetRandomFileName());
                using (FileStream fs = File.Create(tempFilePath))
                {
                    fs.Close();
                }

                File.Delete(tempFilePath);

                return true;
            }
            else
            {
                if (!Directory.Exists(dirPath))
                    throw new DirectoryNotFoundException($"Directory '{dirPath}' not found.");

                string tempFilePath = Path.Combine(dirPath, Path.GetRandomFileName());
                File.WriteAllBytes(tempFilePath, new byte[0]);

                File.Delete(tempFilePath);

                return true;
            }
        }
        catch (Exception ex)
        {
            if (throwIfFails)
                throw new IOException($"Failed to check directory writability for '{dirPath}'. See inner exception for details.", ex);
            else
                return false;
        }
    }
    public static class OperatingSystem
    {
        public static bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }
    }
    
}