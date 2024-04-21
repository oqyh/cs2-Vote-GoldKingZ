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
        private static readonly string jsonFilePath = "VoteGameMode.json";
        private static readonly string jsonFilePath2 = "VoteMap.json";
        private static string? _configFilePath;
        private static string? _jsonFilePath;
        private static string? _jsonFilePath2;
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

        public static ConfigData Load(string modulePath, string GameDirectory)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            var configFolder = Path.Combine(GameDirectory, "csgo/cfg/Vote-GoldKingZ/");
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            if(!Directory.Exists(configFolder) || !Directory.EnumerateFileSystemEntries(configFolder).Any())
            {
                Directory.CreateDirectory(configFolder);
                string content1vs1 = @"//Here is Example Will Execute after Vote Mode 1vs1 successfully Voted
                sv_cheats 0";
                string contentComp = @"//Here is Example Will Execute after Vote Mode Competitive successfully Voted
                sv_cheats 0";
                content1vs1 = Helper.RemoveLeadingSpaces(content1vs1);
                contentComp = Helper.RemoveLeadingSpaces(contentComp);
                string filePath1vs1 = Path.Combine(configFolder, "1vs1.cfg");
                string filePathComp = Path.Combine(configFolder, "Comp.cfg");
                File.WriteAllText(filePath1vs1, content1vs1);
                File.WriteAllText(filePathComp, contentComp);
            }

            _jsonFilePath = Path.Combine(configFileDirectory, jsonFilePath);
            Helper.CreateDefaultWeaponsJson(_jsonFilePath);
            
            _jsonFilePath2 = Path.Combine(configFileDirectory, jsonFilePath2);
            Helper.CreateDefaultWeaponsJson2(_jsonFilePath2);

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
            public string VoteAdmin_CommandsInGame { get; set; }
            public string VoteAdmin_Groups { get; set; }
            public string empty { get; set; }
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
            public bool VoteKick_ChangeTimeInMinsToDays { get; set; }
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
            public string empty2 { get; set; }


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
            public int VoteBanned_TimeInMins { get; set; }
            public bool VoteBanned_ChangeTimeInMinsToDays { get; set; }
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
            public string empty3 { get; set; }

            public bool VoteMute { get; set; }
            public int VoteMute_TimeInMins { get; set; }
            public bool VoteMute_ChangeTimeInMinsToDays { get; set; }
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


            public string empty4 { get; set; }
            public bool VoteGag { get; set; }
            public int VoteGag_TimeInMins { get; set; }
            public bool VoteGag_ChangeTimeInMinsToDays { get; set; }
            public int VoteGag_StartOnMinimumOfXPlayers { get; set; }
            public bool VoteGag_RemoveGagedPlayersOnMapChange { get; set; }
            public bool VoteGag_TeamOnly { get; set; }
            public int VoteGag_Percentage { get; set; }
            public bool VoteGag_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public int VoteGag_CenterMessageAnnouncementTimer { get; set; }
            public bool VoteGag_EvasionPunishment { get; set; }
            public int VoteGag_EvasionPunishmentTimeInMins { get; set; }
            public string VoteGag_CommandsToVote { get; set; }
            public string VoteGag_CommandsOnHalfVoteAccept { get; set; }
            public string VoteGag_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteGag_LetTheseAllowedForGagedPlayers { get; set; }
            public string VoteGag_ImmunityGroups { get; set; }
            public string VoteGag_DisableItOnJoinTheseGroups { get; set; }
            public string empty5 { get; set; }
            public bool VoteSilent { get; set; }
            public int VoteSilent_TimeInMins { get; set; }
            public bool VoteSilent_ChangeTimeInMinsToDays { get; set; }
            public int VoteSilent_StartOnMinimumOfXPlayers { get; set; }
            public bool VoteSilent_RemoveSilentedPlayersOnMapChange { get; set; }
            public bool VoteSilent_TeamOnly { get; set; }
            public int VoteSilent_Percentage { get; set; }
            public bool VoteSilent_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public int VoteSilent_CenterMessageAnnouncementTimer { get; set; }
            public bool VoteSilent_EvasionPunishment { get; set; }
            public int VoteSilent_EvasionPunishmentTimeInMins { get; set; }
            public string VoteSilent_CommandsToVote { get; set; }
            public string VoteSilent_CommandsOnHalfVoteAccept { get; set; }
            public string VoteSilent_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteSilent_LetTheseAllowedForSilentedPlayers { get; set; }
            public string VoteSilent_ImmunityGroups { get; set; }
            public string VoteSilent_DisableItOnJoinTheseGroups { get; set; }
            public string empty6 { get; set; }
            public bool VoteGameMode { get; set; }
            public int VoteGameMode_StartOnMinimumOfXPlayers { get; set; }
            public int VoteGameMode_Percentage { get; set; }
            public bool VoteGameMode_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public float VoteGameMode_CenterMessageAnnouncementTimer { get; set; }
            public string VoteGameMode_CommandsToVote { get; set; }
            public string VoteGameMode_CommandsOnHalfVoteAccept { get; set; }
            public string VoteGameMode_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteGameMode_DisableItOnJoinTheseGroups { get; set; }
            public string empty7 { get; set; }
            public bool VoteMap { get; set; }
            public int VoteMap_StartOnMinimumOfXPlayers { get; set; }
            public int VoteMap_Percentage { get; set; }
            public bool VoteMap_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public float VoteMap_CenterMessageAnnouncementTimer { get; set; }
            public string VoteMap_CommandsToVote { get; set; }
            public string VoteMap_CommandsOnHalfVoteAccept { get; set; }
            public string VoteMap_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteMap_DisableItOnJoinTheseGroups { get; set; }            
            public string empty8 { get; set; }
            public bool Log_SendLogToText { get; set; }
            public string Log_TextMessageFormat { get; set; }
            public string Log_GameModeFormat { get; set; }
            public string Log_MapFormat { get; set; }
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
            public string Log_DiscordGameModeFormat { get; set; }
            public string Log_DiscordMapFormat { get; set; }
            public string Log_DiscordUsersWithNoAvatarImage { get; set; }
            public string empty9 { get; set; }
            public string Information_For_You_Dont_Delete_it { get; set; }
            
            public ConfigData()
            {
                VoteAdmin_CommandsInGame = "!votesadmin,!voteadmin";
                VoteAdmin_Groups = "@css/root,@css/admin";
                empty = "-----------------------------------------------------------------------------------";
                VoteKick_Mode = 2;
                VoteKick_TimeInMins = 5;
                VoteKick_ChangeTimeInMinsToDays = false;
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
                empty2 = "-----------------------------------------------------------------------------------";
                VoteBanned_Mode = 0;
                VoteBanned_TimeInMins = 1;
                VoteBanned_ChangeTimeInMinsToDays = true;
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
                empty3 = "-----------------------------------------------------------------------------------";
                VoteMute = false;
                VoteMute_TimeInMins = 5;
                VoteMute_ChangeTimeInMinsToDays = false;
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
                empty4 = "-----------------------------------------------------------------------------------";
                VoteGag = false;
                VoteGag_TimeInMins = 5;
                VoteGag_ChangeTimeInMinsToDays = false;
                VoteGag_StartOnMinimumOfXPlayers  = 5;
                VoteGag_RemoveGagedPlayersOnMapChange = false;
                VoteGag_TeamOnly = false;
                VoteGag_Percentage = 60;
                VoteGag_CenterMessageAnnouncementOnHalfVotes = false;
                VoteGag_CenterMessageAnnouncementTimer = 25;
                VoteGag_EvasionPunishment = false;
                VoteGag_EvasionPunishmentTimeInMins = 10;
                VoteGag_CommandsToVote = "!votegag,!gag,!vg";
                VoteGag_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteGag_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteGag_LetTheseAllowedForGagedPlayers = "!,.,@";
                VoteGag_ImmunityGroups = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip";
                VoteGag_DisableItOnJoinTheseGroups = "";
                empty5 = "-----------------------------------------------------------------------------------";
                VoteSilent = false;
                VoteSilent_TimeInMins = 5;
                VoteSilent_ChangeTimeInMinsToDays = false;
                VoteSilent_StartOnMinimumOfXPlayers  = 5;
                VoteSilent_RemoveSilentedPlayersOnMapChange = false;
                VoteSilent_TeamOnly = false;
                VoteSilent_Percentage = 60;
                VoteSilent_CenterMessageAnnouncementOnHalfVotes = false;
                VoteSilent_CenterMessageAnnouncementTimer = 25;
                VoteSilent_EvasionPunishment = false;
                VoteSilent_EvasionPunishmentTimeInMins = 10;
                VoteSilent_CommandsToVote = "!votesilent,!slt,!vs";
                VoteSilent_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteSilent_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteSilent_LetTheseAllowedForSilentedPlayers = "!,.,@";
                VoteSilent_ImmunityGroups = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip";
                VoteSilent_DisableItOnJoinTheseGroups = "";
                empty6 = "-----------------------------------------------------------------------------------";
                VoteGameMode = false;
                VoteGameMode_StartOnMinimumOfXPlayers  = 5;
                VoteGameMode_Percentage = 60;
                VoteGameMode_CenterMessageAnnouncementOnHalfVotes = false;
                VoteGameMode_CenterMessageAnnouncementTimer = 25;
                VoteGameMode_CommandsToVote = "!votemodes,!votemode";
                VoteGameMode_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteGameMode_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteGameMode_DisableItOnJoinTheseGroups = "";
                empty7 = "-----------------------------------------------------------------------------------";
                VoteMap = false;
                VoteMap_StartOnMinimumOfXPlayers  = 5;
                VoteMap_Percentage = 60;
                VoteMap_CenterMessageAnnouncementOnHalfVotes = false;
                VoteMap_CenterMessageAnnouncementTimer = 25;
                VoteMap_CommandsToVote = "!votemaps,!votemap";
                VoteMap_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteMap_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteMap_DisableItOnJoinTheseGroups = "";                
                empty8 = "-----------------------------------------------------------------------------------";
                Log_SendLogToText = false;
                Log_TextMessageFormat = "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]";
                Log_GameModeFormat = "[{DATE} - {TIME}] Vote Game Mode Choosed To Change To ({GAMEMODE})";
                Log_MapFormat = "[{DATE} - {TIME}] Vote Map Choosed To Change To ({MAP})";
                Log_AutoDeleteLogsMoreThanXdaysOld = 0;
                Log_SendLogToDiscordOnMode = 0;
                Log_DiscordSideColor = "00FFFF";
                Log_DiscordWebHookURL = "https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                Log_DiscordMessageFormat = "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]";
                Log_DiscordGameModeFormat = "[{DATE} - {TIME}] Vote Game Mode Choosed To Change To ({GAMEMODE})";
                Log_DiscordMapFormat = "[{DATE} - {TIME}] Vote Map Choosed To Change To ({MAP})";
                Log_DiscordUsersWithNoAvatarImage = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/b5/b5bd56c1aa4644a474a2e4972be27ef9e82e517e_full.jpg";
                empty9 = "-----------------------------------------------------------------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/oqyh/cs2-Vote-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-] To Understand All Above";
            }
        }
    }
}
