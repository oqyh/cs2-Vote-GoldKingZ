using System.Text.Json;
using System.Text.Json.Serialization;

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
            public bool VoteKick_AllowKickedPlayersToJoinOnMapChange { get; set; }
            public bool VoteKick_TeamOnly { get; set; }
            public float VoteKick_Percentage { get; set; }
            public bool VoteKick_CenterMessageAnnouncementOnHalfVotes { get; set; }
            public float VoteKick_CenterMessageAnnouncementTimer { get; set; }
            public bool VoteKick_EvasionPunishment { get; set; }
            public int VoteKick_EvasionPunishmentTimeInMins { get; set; }
            public string VoteKick_CommandsToVote { get; set; }
            public string VoteKick_CommandsOnHalfVoteAccept { get; set; }
            public string VoteKick_CommandsOnHalfVoteRefuse { get; set; }
            public string VoteKick_ImmunityGroups { get; set; }
            public string Info_AboutAllAbove { get; set; }
            
            public ConfigData()
            {
                VoteKick_Mode = 2;
                VoteKick_TimeInMins = 5;
                VoteKick_AllowKickedPlayersToJoinOnMapChange = false;
                VoteKick_TeamOnly = false;
                VoteKick_Percentage = 6;
                VoteKick_CenterMessageAnnouncementOnHalfVotes = false;
                VoteKick_CenterMessageAnnouncementTimer = 25;
                VoteKick_EvasionPunishment = false;
                VoteKick_EvasionPunishmentTimeInMins = 10;
                VoteKick_CommandsToVote = "!votekick,!kick,!vk";
                VoteKick_CommandsOnHalfVoteAccept = "!yes,yes,!y,y";
                VoteKick_CommandsOnHalfVoteRefuse = "!no,no,!n,n";
                VoteKick_ImmunityGroups = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip";
                Info_AboutAllAbove = " For More Info Vist  [https://github.com/oqyh/cs2-Vote-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-]";
            }
        }
    }
}