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
using System.Text.Json;

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
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.CounterTerrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.Terrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected).ToList();
        return playerList;
    }
    public static int GetCounterTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.CounterTerrorist);
    }
    public static int GetTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.Terrorist);
    }
    public static int GetAllCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected);
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
        Globals_VoteKick.VoteKick_Disabled = false;
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
        Globals_VoteKick.VoteKick_Disable.Clear();
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
        Globals_VoteBanned.VoteBanned_Disabled = false;
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
        Globals_VoteBanned.VoteBanned_Disable.Clear();
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
        Globals_VoteMute.VoteMute_Disabled = false;
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
        Globals_VoteMute.VoteMute_Disable.Clear();
        Globals_VoteMute.VoteMute_GetVoted.Clear();
        Globals_VoteMute.VoteMute_CallerVotedTo.Clear();
    }
    public static void ClearVariablesVoteGag()
    {
        Globals_VoteGag.VoteGag_timerCT = 0f;
        Globals_VoteGag.VoteGag_timerT = 0f;
        Globals_VoteGag.VoteGag_timerBOTH = 0f;
        Globals_VoteGag.VoteGag_targetPlayerNameCT = "";
        Globals_VoteGag.VoteGag_targetPlayerNameT = "";
        Globals_VoteGag.VoteGag_targetPlayerNameBOTH = "";
        Globals_VoteGag.VoteGag_targetPlayerIPCT = "";
        Globals_VoteGag.VoteGag_targetPlayerIPT = "";
        Globals_VoteGag.VoteGag_targetPlayerIPBOTH = "";
        Globals_VoteGag.VoteGag_targetPlayerSTEAMCT = 0;
        Globals_VoteGag.VoteGag_targetPlayerSTEAMT = 0;
        Globals_VoteGag.VoteGag_targetPlayerSTEAMBOTH = 0;
        Globals_VoteGag.VoteGag_Disabled = false;
        Globals_VoteGag.VoteGag_ReachHalfVoteCT = false;
        Globals_VoteGag.VoteGag_ReachHalfVoteT = false;
        Globals_VoteGag.VoteGag_ReachHalfVoteBoth = false;
        Globals_VoteGag.VoteGag_countingCT = 0;
        Globals_VoteGag.VoteGag_countingT = 0;
        Globals_VoteGag.VoteGag_countingBoth = 0;
        Globals_VoteGag.VoteGag_requiredct = 0;
        Globals_VoteGag.VoteGag_requiredt = 0;
        Globals_VoteGag.VoteGag_requiredboth = 0;
        Globals_VoteGag.VoteGag_ShowMenuCT.Clear();
        Globals_VoteGag.VoteGag_ShowMenuT.Clear();
        Globals_VoteGag.VoteGag_ShowMenuBOTH.Clear();
        Globals_VoteGag.VoteGag_Immunity.Clear();
        Globals_VoteGag.VoteGag_Disable.Clear();
        Globals_VoteGag.VoteGag_GetVoted.Clear();
        Globals_VoteGag.VoteGag_PlayerGaged.Clear();
        Globals_VoteGag.VoteGag_CallerVotedTo.Clear();
    }
    public static void ClearVariablesVoteSilent()
    {
        Globals_VoteSilent.VoteSilent_timerCT = 0f;
        Globals_VoteSilent.VoteSilent_timerT = 0f;
        Globals_VoteSilent.VoteSilent_timerBOTH = 0f;
        Globals_VoteSilent.VoteSilent_targetPlayerNameCT = "";
        Globals_VoteSilent.VoteSilent_targetPlayerNameT = "";
        Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH = "";
        Globals_VoteSilent.VoteSilent_targetPlayerIPCT = "";
        Globals_VoteSilent.VoteSilent_targetPlayerIPT = "";
        Globals_VoteSilent.VoteSilent_targetPlayerIPBOTH = "";
        Globals_VoteSilent.VoteSilent_targetPlayerSTEAMCT = 0;
        Globals_VoteSilent.VoteSilent_targetPlayerSTEAMT = 0;
        Globals_VoteSilent.VoteSilent_targetPlayerSTEAMBOTH = 0;
        Globals_VoteSilent.VoteSilent_Disabled = false;
        Globals_VoteSilent.VoteSilent_ReachHalfVoteCT = false;
        Globals_VoteSilent.VoteSilent_ReachHalfVoteT = false;
        Globals_VoteSilent.VoteSilent_ReachHalfVoteBoth = false;
        Globals_VoteSilent.VoteSilent_countingCT = 0;
        Globals_VoteSilent.VoteSilent_countingT = 0;
        Globals_VoteSilent.VoteSilent_countingBoth = 0;
        Globals_VoteSilent.VoteSilent_requiredct = 0;
        Globals_VoteSilent.VoteSilent_requiredt = 0;
        Globals_VoteSilent.VoteSilent_requiredboth = 0;
        Globals_VoteSilent.VoteSilent_ShowMenuCT.Clear();
        Globals_VoteSilent.VoteSilent_ShowMenuT.Clear();
        Globals_VoteSilent.VoteSilent_ShowMenuBOTH.Clear();
        Globals_VoteSilent.VoteSilent_Immunity.Clear();
        Globals_VoteSilent.VoteSilent_Disable.Clear();
        Globals_VoteSilent.VoteSilent_GetVoted.Clear();
        Globals_VoteSilent.VoteSilent_PlayerGaged.Clear();
        Globals_VoteSilent.VoteSilent_CallerVotedTo.Clear();
    }
    public static void ClearVariablesVoteGameMode()
    {
        Globals_VoteGameMode.VoteGameMode_timerBOTH = 0f;
        Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH = "";
        Globals_VoteGameMode.VoteGameMode_targetPlayerIPBOTH = "";
        Globals_VoteGameMode.VoteGameMode_targetPlayerSTEAMBOTH = 0;
        Globals_VoteGameMode.VoteGameMode_Disabled = false;
        Globals_VoteGameMode.VoteGameMode_ReachHalfVoteBoth = false;
        Globals_VoteGameMode.VoteGameMode_countingBoth = 0;
        Globals_VoteGameMode.VoteGameMode_requiredboth = 0;
        Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.Clear();
        Globals_VoteGameMode.VoteGameMode_Immunity.Clear();
        Globals_VoteGameMode.VoteGameMode_Disable.Clear();
        Globals_VoteGameMode.VoteGameMode_GetVoted.Clear();
        Globals_VoteGameMode.VoteGameMode_CallerVotedTo.Clear();
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
    public static string ReplaceMessagesMode(string Message, string date, string time, string GameModeName)
    {
        var replacedMessage = Message
                                    .Replace("{TIME}", time)
                                    .Replace("{DATE}", date)
                                    .Replace("{GAMEMODE}", GameModeName.ToString());
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
    public static void CreateDefaultWeaponsJson(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            var configData = new Dictionary<string, object>
            {
                { "1vs1", new { Config = "1vs1.cfg" } },
                { "Competitive", new { Config = "Comp.cfg" } }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(configData, options);

            File.WriteAllText(jsonFilePath, json);
        }
    }
    public static string RemoveLeadingSpaces(string content)
    {
        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimStart();
        }
        return string.Join("\n", lines);
    }
}