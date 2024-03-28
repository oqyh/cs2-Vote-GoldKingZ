using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

namespace Vote_GoldKingZ.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesFolderPath { get; set; }
        }
        
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
            }
            else
            {
                _configData = new ConfigData();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);

            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
            {
                throw new Exception("Config not yet loaded.");
            }

            File.WriteAllText(_configFilePath, JsonSerializer.Serialize(configData, SerializationOptions));
        }

        public class ConfigData
        {
            private int _voteKickMode;
            public int VoteKick_Mode
            {
                get => _voteKickMode;
                set
                {
                    _voteKickMode = value;
                    if (_voteKickMode < 0 || _voteKickMode > 4)
                    {
                        VoteKick_Mode = 2;
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                        Console.WriteLine("[Vote-GoldKingZ] VoteKick_Mode: is invalid, setting to default value (2) Please Choose 0 or 1 or 2 or 3 or 4.");
                        Console.WriteLine("[Vote-GoldKingZ] VoteKick_Mode (0) = Disable");
                        Console.WriteLine("[Vote-GoldKingZ] VoteKick_Mode (1) = Kick Only");
                        Console.WriteLine("[Vote-GoldKingZ] VoteKick_Mode (2) = Kick And Restrict SteamID From Joining");
                        Console.WriteLine("[Vote-GoldKingZ] VoteKick_Mode (3) = Kick And Restrict IpAddress From Joining");
                        Console.WriteLine("[Vote-GoldKingZ] VoteKick_Mode (4) = Kick And Restrict SteamID And IpAddress From Joining");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    }
                }
            }
            public int VoteKick_TimeInMins { get; set; }
            public int VoteKick_StartOnMinimumOfXPlayers { get; set; }
            public bool VoteKick_AllowKickedPlayersToJoinOnMapChange { get; set; }
            public bool VoteKick_TeamOnly { get; set; }
            public float VoteKick_Percentage { get; set; }
            public bool VoteKick_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public float VoteKick_CenterMessageAnnouncementTimer { get; set; }
            public bool VoteKick_EvasionPunishment { get; set; }
            public int VoteKick_EvasionPunishmentTimeInMins { get; set; }
            public bool VoteKick_DelayKick { get; set; }
            public string VoteKick_CommandsToVote { get; set; }
            public string VoteKick_CommandsOnHalfVoteAccept { get; set; }
            public string VoteKick_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteKick_ImmunityGroups { get; set; }
            public string VoteKick_DisableItOnJoinTheseGroups { get; set; }
            public string empty { get; set; }


            private int _voteBannedMode;
            public int VoteBanned_Mode
            {
                get => _voteBannedMode;
                set
                {
                    _voteBannedMode = value;
                    if (_voteBannedMode < 0 || _voteBannedMode > 3)
                    {
                        VoteBanned_Mode = 0;
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                        Console.WriteLine("[Vote-GoldKingZ] VoteBanned_Mode: is invalid, setting to default value (0) Please Choose 0 or 1 or 2 or 3.");
                        Console.WriteLine("[Vote-GoldKingZ] VoteBanned_Mode (0) = Disable");
                        Console.WriteLine("[Vote-GoldKingZ] VoteBanned_Mode (1) = Banned And Restrict SteamID From Joining");
                        Console.WriteLine("[Vote-GoldKingZ] VoteBanned_Mode (2) = Banned And Restrict IpAddress From Joining");
                        Console.WriteLine("[Vote-GoldKingZ] VoteBanned_Mode (3) = Banned And Restrict SteamID And IpAddress From Joining");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    }
                }
            }
            public int VoteBanned_TimeInDays { get; set; }
            public int VoteBanned_StartOnMinimumOfXPlayers { get; set; }
            public bool VoteBanned_TeamOnly { get; set; }
            public float VoteBanned_Percentage { get; set; }
            public bool VoteBanned_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public float VoteBanned_CenterMessageAnnouncementTimer { get; set; }
            public bool VoteBanned_EvasionPunishment { get; set; }
            public int VoteBanned_EvasionPunishmentTimeInDays { get; set; }
            public bool VoteBanned_DelayKick { get; set; }
            public string VoteBanned_CommandsToVote { get; set; }
            public string VoteBanned_CommandsOnHalfVoteAccept { get; set; }
            public string VoteBanned_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteBanned_ImmunityGroups { get; set; }
            public string VoteBanned_DisableItOnJoinTheseGroups { get; set; }
            public string empty2 { get; set; }

            public bool VoteMute { get; set; }
            public int VoteMute_TimeInMins { get; set; }
            public int VoteMute_StartOnMinimumOfXPlayers { get; set; }
            public bool VoteMute_RemoveMutedPlayersOnMapChange { get; set; }
            public bool VoteMute_TeamOnly { get; set; }
            public int VoteMute_Percentage { get; set; }
            public bool VoteMute_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public int VoteMute_CenterMessageAnnouncementTimer { get; set; }
            public bool VoteMute_EvasionPunishment { get; set; }
            public int VoteMute_EvasionPunishmentTimeInMins { get; set; }
            public string VoteMute_CommandsToVote { get; set; }
            public string VoteMute_CommandsOnHalfVoteAccept { get; set; }
            public string VoteMute_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteMute_ImmunityGroups { get; set; }
            public string VoteMute_DisableItOnJoinTheseGroups { get; set; }


            public string empty3 { get; set; }
            public bool Log_SendLogToText { get; set; }
            public string Log_TextMessageFormat { get; set; }
            public int Log_AutoDeleteLogsMoreThanXdaysOld { get; set; }
            private int _Log_SendLogToDiscordOnMode;
            public int Log_SendLogToDiscordOnMode
            {
                get => _Log_SendLogToDiscordOnMode;
                set
                {
                    _Log_SendLogToDiscordOnMode = value;
                    if (_Log_SendLogToDiscordOnMode < 0 || _Log_SendLogToDiscordOnMode > 3)
                    {
                        Log_SendLogToDiscordOnMode = 0;
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                        Console.WriteLine("[Vote-GoldKingZ] Log_SendLogToDiscordOnMode: is invalid, setting to default value (0) Please Choose 0 or 1 or 2 or 3.");
                        Console.WriteLine("[Vote-GoldKingZ] Log_SendLogToDiscordOnMode (0) = Disable");
                        Console.WriteLine("[Vote-GoldKingZ] Log_SendLogToDiscordOnMode (1) = Text Only");
                        Console.WriteLine("[Vote-GoldKingZ] Log_SendLogToDiscordOnMode (2) = Text With + Name + Hyperlink To Steam Profile");
                        Console.WriteLine("[Vote-GoldKingZ] Log_SendLogToDiscordOnMode (3) = Text With + Name + Hyperlink To Steam Profile + Profile Picture");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    }
                }
            }
            private string? _Log_DiscordSideColor;
            public string Log_DiscordSideColor
            {
                get => _Log_DiscordSideColor!;
                set
                {
                    _Log_DiscordSideColor = value;
                    if (_Log_DiscordSideColor.StartsWith("#"))
                    {
                        Log_DiscordSideColor = _Log_DiscordSideColor.Substring(1);
                    }
                }
            }
            public string Log_DiscordWebHookURL { get; set; }
            public string Log_DiscordMessageFormat { get; set; }
            public string Log_DiscordUsersWithNoAvatarImage { get; set; }
            public string empty4 { get; set; }
            public string Information_For_You_Dont_Delete_it { get; set; }
            
            public ConfigData()
            {
                VoteKick_Mode = 2;
                VoteKick_TimeInMins = 5;
                VoteKick_StartOnMinimumOfXPlayers  = 5;
                VoteKick_AllowKickedPlayersToJoinOnMapChange = false;
                VoteKick_TeamOnly = false;
                VoteKick_Percentage = 60;
                VoteKick_CenterMessageAnnouncementOnHalfVotes = false;
                VoteKick_CenterMessageAnnouncementTimer = 25;
                VoteKick_EvasionPunishment = false;
                VoteKick_EvasionPunishmentTimeInMins = 10;
                VoteKick_DelayKick = false;
                VoteKick_CommandsToVote = "!votekick,!kick,!vk";
                VoteKick_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteKick_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteKick_ImmunityGroups = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip";
                VoteKick_DisableItOnJoinTheseGroups = "";
                empty = "-----------------------------------------------------------------------------------";
                VoteBanned_Mode = 0;
                VoteBanned_TimeInDays = 1;
                VoteBanned_StartOnMinimumOfXPlayers  = 8;
                VoteBanned_TeamOnly = false;
                VoteBanned_Percentage = 80;
                VoteBanned_CenterMessageAnnouncementOnHalfVotes = false;
                VoteBanned_CenterMessageAnnouncementTimer = 25;
                VoteBanned_EvasionPunishment = false;
                VoteBanned_EvasionPunishmentTimeInDays = 2;
                VoteBanned_DelayKick = false;
                VoteBanned_CommandsToVote = "!votebanned,!banned,!vb";
                VoteBanned_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteBanned_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteBanned_ImmunityGroups = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip";
                VoteBanned_DisableItOnJoinTheseGroups = "";
                empty2 = "-----------------------------------------------------------------------------------";
                VoteMute = false;
                VoteMute_TimeInMins = 5;
                VoteMute_StartOnMinimumOfXPlayers  = 5;
                VoteMute_RemoveMutedPlayersOnMapChange = false;
                VoteMute_TeamOnly = false;
                VoteMute_Percentage = 60;
                VoteMute_CenterMessageAnnouncementOnHalfVotes = false;
                VoteMute_CenterMessageAnnouncementTimer = 25;
                VoteMute_EvasionPunishment = false;
                VoteMute_EvasionPunishmentTimeInMins = 10;
                VoteMute_CommandsToVote = "!votemute,!mute,!vm";
                VoteMute_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteMute_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteMute_ImmunityGroups = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip";
                VoteMute_DisableItOnJoinTheseGroups = "";
                empty3 = "-----------------------------------------------------------------------------------";
                Log_SendLogToText = false;
                Log_TextMessageFormat = "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]";
                Log_AutoDeleteLogsMoreThanXdaysOld = 0;
                Log_SendLogToDiscordOnMode = 0;
                Log_DiscordSideColor = "00FFFF";
                Log_DiscordWebHookURL = "https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                Log_DiscordMessageFormat = "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]";
                Log_DiscordUsersWithNoAvatarImage = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/b5/b5bd56c1aa4644a474a2e4972be27ef9e82e517e_full.jpg";
                empty4 = "-----------------------------------------------------------------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/oqyh/cs2-Vote-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-] To Understand All Above";
            }
        }
    }
}
