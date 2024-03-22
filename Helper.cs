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
    public class PersonData
    {
        public ulong PlayerSteamID { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerIPAddress { get; set; }
        public DateTime DateAndTime { get; set; }
        public int RestrictedFor { get; set; }
        public string? Reason { get; set; }
    }

    public static void SaveToJsonFile(ulong PlayerSteamID, string PlayerName, string PlayerIPAddress, DateTime DateAndTime, bool days, int RestrictedFor, string Reason, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (!Directory.Exists(Fpath))
            {
                Directory.CreateDirectory(Fpath);
            }

            if (!File.Exists(Fpathc))
            {
                File.WriteAllText(Fpathc, "[]");
            }

            List<PersonData> allPersonsData;
            string jsonData = File.ReadAllText(Fpathc);
            allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

            PersonData existingPerson = allPersonsData.Find(p => p.PlayerSteamID == PlayerSteamID)!;

            if (existingPerson != null)
            {
                existingPerson.DateAndTime = DateAndTime;
                existingPerson.Reason = Reason;
            }
            else
            {
                PersonData newPerson = new PersonData { PlayerSteamID = PlayerSteamID, PlayerName = PlayerName, PlayerIPAddress = PlayerIPAddress, DateAndTime = DateAndTime, RestrictedFor = RestrictedFor, Reason = Reason };
                allPersonsData.Add(newPerson);
            }

            int day = RestrictedFor; 
            double interval = days ? day : day; 

            allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalMinutes > interval);

            string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
            try
            {
                File.WriteAllText(Fpathc, updatedJsonData);
            }
            catch
            {
                // Handle exception
            }
        }
        catch
        {
            // Handle exception
        }
    }

    public static PersonData RetrievePersonDataById(ulong targetId, bool days, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.PlayerSteamID == targetId)!;

                int day = Time; 
                double interval = days ? day : day;
                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime).TotalMinutes <= interval)
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }
    public static PersonData RetrievePersonDataByIp(string PlayerIPAddress, bool days, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.PlayerIPAddress == PlayerIPAddress)!;

                int day = Time; 
                double interval = days ? day : day;
                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime).TotalMinutes <= interval)
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }
    public static PersonData RetrievePersonDataByReason(string reason, bool days, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.Reason == reason)!;

                int day = Time; 
                double interval = days ? day : day;
                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime).TotalMinutes <= interval)
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }
    public static void RemoveAnyByReason(string Reason, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                List<PersonData> kickedPersons = allPersonsData.Where(p => p.Reason == Reason).ToList();

                foreach (var kickedPerson in kickedPersons)
                {
                    allPersonsData.Remove(kickedPerson);
                }

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(Fpathc, updatedJsonData);
                }
                catch
                {
                    // Handle exception
                }
            }
        }
        catch
        {
            // Handle exception
        }
    }
    public static bool IsPlayerIPRestricted(string PlayerIPAddress, bool days, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();
                int day = Time; 
                double interval = days ? day : day; 

                allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalMinutes > interval);

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(Fpathc, updatedJsonData);
                }
                catch
                {
                    // Handle exception
                }

                foreach (var personData in allPersonsData)
                {
                    if (personData.PlayerIPAddress != null && personData.PlayerIPAddress == PlayerIPAddress)
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return false;
    }
    public static bool IsPlayerSteamIDRestricted(ulong PlayerId, bool days, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();
                int day = Time; 
                double interval = days ? day : day; 

                allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalMinutes > interval);

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(Fpathc, updatedJsonData);
                }
                catch
                {
                    // Handle exception
                }
                foreach (var personData in allPersonsData)
                {
                    if (personData.PlayerIPAddress != null && personData.PlayerSteamID == PlayerId)
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return false;
    }

    public static void ClearVariablesVoteKick()
    {
        Globals.VoteKick_timerCT = 0f;
        Globals.VoteKick_timerT = 0f;
        Globals.VoteKick_timerBOTH = 0f;
        Globals.VoteKick_targetPlayerNameCT = "";
        Globals.VoteKick_targetPlayerNameT = "";
        Globals.VoteKick_targetPlayerNameBOTH = "";
        Globals.VoteKick_targetPlayerIPCT = "";
        Globals.VoteKick_targetPlayerIPT = "";
        Globals.VoteKick_targetPlayerIPBOTH = "";
        Globals.VoteKick_targetPlayerSTEAMCT = 0;
        Globals.VoteKick_targetPlayerSTEAMT = 0;
        Globals.VoteKick_targetPlayerSTEAMBOTH = 0;
        Globals.VoteKick_ReachHalfVoteCT = false;
        Globals.VoteKick_ReachHalfVoteT = false;
        Globals.VoteKick_ReachHalfVoteBoth = false;
        Globals.VoteKick_countingCT = 0;
        Globals.VoteKick_countingT = 0;
        Globals.VoteKick_countingBoth = 0;
        Globals.VoteKick_requiredct = 0;
        Globals.VoteKick_requiredt = 0;
        Globals.VoteKick_requiredboth = 0;
        Globals.VoteKick_ShowMenuCT.Clear();
        Globals.VoteKick_ShowMenuT.Clear();
        Globals.VoteKick_ShowMenuBOTH.Clear();
        Globals.VoteKick_Immunity.Clear();
        Globals.VoteKick_GetVoted.Clear();
        Globals.VoteKick_CallerVotedTo.Clear();
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