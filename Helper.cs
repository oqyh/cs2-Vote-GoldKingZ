using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using Vote_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Entities;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text;
using System.Drawing;

namespace Vote_GoldKingZ;

public class Helper
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly HttpClient httpClient = new HttpClient();

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
    public static void ClearVariablesVoteBan()
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
    public static void ClearVariablesVoteMute()
    {
        Globals_VoteMute.VoteMute_timerCT = 0f;
        Globals_VoteMute.VoteMute_timerT = 0f;
        Globals_VoteMute.VoteMute_timerBOTH = 0f;
        Globals_VoteMute.VoteMute_targetPlayerNameCT = "";
        Globals_VoteMute.VoteMute_targetPlayerNameT = "";
        Globals_VoteMute.VoteMute_targetPlayerNameBOTH = "";
        Globals_VoteMute.VoteMute_targetPlayerIPCT = "";
        Globals_VoteMute.VoteMute_targetPlayerIPT = "";
        Globals_VoteMute.VoteMute_targetPlayerIPBOTH = "";
        Globals_VoteMute.VoteMute_targetPlayerSTEAMCT = 0;
        Globals_VoteMute.VoteMute_targetPlayerSTEAMT = 0;
        Globals_VoteMute.VoteMute_targetPlayerSTEAMBOTH = 0;
        Globals_VoteMute.VoteMute_ReachHalfVoteCT = false;
        Globals_VoteMute.VoteMute_ReachHalfVoteT = false;
        Globals_VoteMute.VoteMute_ReachHalfVoteBoth = false;
        Globals_VoteMute.VoteMute_countingCT = 0;
        Globals_VoteMute.VoteMute_countingT = 0;
        Globals_VoteMute.VoteMute_countingBoth = 0;
        Globals_VoteMute.VoteMute_requiredct = 0;
        Globals_VoteMute.VoteMute_requiredt = 0;
        Globals_VoteMute.VoteMute_requiredboth = 0;
        Globals_VoteMute.VoteMute_ShowMenuCT.Clear();
        Globals_VoteMute.VoteMute_ShowMenuT.Clear();
        Globals_VoteMute.VoteMute_ShowMenuBOTH.Clear();
        Globals_VoteMute.VoteMute_Immunity.Clear();
        Globals_VoteMute.VoteMute_GetVoted.Clear();
        Globals_VoteMute.VoteMute_CallerVotedTo.Clear();
    }
    
    public static string ReplaceMessages(string Message, string date, string time, string PlayerName, string SteamId, string ipAddress, string reason)
    {
        var replacedMessage = Message
                                    .Replace("{TIME}", time)
                                    .Replace("{DATE}", date)
                                    .Replace("{PLAYERNAME}", PlayerName.ToString())
                                    .Replace("{STEAMID}", SteamId.ToString())
                                    .Replace("{IP}", ipAddress.ToString())
                                    .Replace("{REASON}", reason);
        return replacedMessage;
    }
    public static async Task SendToDiscordWebhookNormal(string webhookUrl, string message)
    {
        try
        {
            var payload = new { content = message };
            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);

            
        }
        catch
        {
        }
    }

    public static async Task SendToDiscordWebhookNameLink(string webhookUrl, string message, string steamUserId, string STEAMNAME)
    {
        try
        {
            string profileLink = GetSteamProfileLink(steamUserId);
            int colorss = int.Parse(Configs.GetConfigData().Log_DiscordSideColor, System.Globalization.NumberStyles.HexNumber);
            Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);
            using (var httpClient = new HttpClient())
            {
                var embed = new
                {
                    type = "rich",
                    title = STEAMNAME,
                    url = profileLink,
                    description = message,
                    color = color.ToArgb() & 0xFFFFFF
                };

                var payload = new
                {
                    embeds = new[] { embed }
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);

            }
        }
        catch 
        {
        }
    }
    public static async Task SendToDiscordWebhookNameLinkWithPicture(string webhookUrl, string message, string steamUserId, string STEAMNAME)
    {
        try
        {
            string profileLink = GetSteamProfileLink(steamUserId);
            string profilePictureUrl = await GetProfilePictureAsync(steamUserId, Configs.GetConfigData().Log_DiscordUsersWithNoAvatarImage);
            int colorss = int.Parse(Configs.GetConfigData().Log_DiscordSideColor, System.Globalization.NumberStyles.HexNumber);
            Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);
            using (var httpClient = new HttpClient())
            {
                var embed = new
                {
                    type = "rich",
                    description = message,
                    color = color.ToArgb() & 0xFFFFFF,
                    author = new
                    {
                        name = STEAMNAME,
                        url = profileLink,
                        icon_url = profilePictureUrl
                    }
                };

                var payload = new
                {
                    embeds = new[] { embed }
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);
            }
        }
        catch
        {

        }
    }
    public static async Task<string> GetProfilePictureAsync(string steamId64, string defaultImage)
    {
        try
        {
            string apiUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string xmlResponse = await response.Content.ReadAsStringAsync();
                int startIndex = xmlResponse.IndexOf("<avatarFull><![CDATA[") + "<avatarFull><![CDATA[".Length;
                int endIndex = xmlResponse.IndexOf("]]></avatarFull>", startIndex);

                if (endIndex >= 0)
                {
                    string profilePictureUrl = xmlResponse.Substring(startIndex, endIndex - startIndex);
                    return profilePictureUrl;
                }
                else
                {
                    return defaultImage;
                }
            }
            else
            {
                return null!;
            }
        }
        catch
        {
            return null!;
        }
    }
    public static string GetSteamProfileLink(string userId)
    {
        return $"https://steamcommunity.com/profiles/{userId}";
    }
    public static void DeleteOldFiles(string folderPath, string searchPattern, TimeSpan maxAge)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] files = directoryInfo.GetFiles(searchPattern);
                DateTime currentTime = DateTime.Now;
                
                foreach (FileInfo file in files)
                {
                    TimeSpan age = currentTime - file.LastWriteTime;

                    if (age > maxAge)
                    {
                        file.Delete();
                    }
                }
            }
            else
            {
                
            }
        }
        catch
        {
        }
    }
    
}
