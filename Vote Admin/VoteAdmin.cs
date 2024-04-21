using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Menu;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Vote_GoldKingZ;

public class VoteAdmin
{
    private IStringLocalizer? Localizer;
    public void SetStringLocalizer(IStringLocalizer stringLocalizer)
    {
        Localizer = stringLocalizer;
    }
    public class PlayerData
    {
        public ulong PlayerSteamID { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerIPAddress { get; set; }
        public DateTime DateAndTime { get; set; }
        public int RestrictedForXMins { get; set; }
        public int RestrictedForXDays { get; set; }
        public string? Reason { get; set; }
    }
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().VoteAdmin_Groups))
        {
            if (!Globals_VoteAdmin.VoteAdmin_Admins.ContainsKey(playerid))
            {
                Globals_VoteAdmin.VoteAdmin_Admins.Add(playerid, true);
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(string.IsNullOrEmpty(Configs.GetConfigData().VoteGameMode_CommandsToVote) || @event == null)return HookResult.Continue;
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
        string[] adminscommands = Configs.GetConfigData().VoteAdmin_CommandsInGame.Split(',');
        
        if (adminscommands.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Configs.GetConfigData().VoteAdmin_Groups) && !Globals_VoteAdmin.VoteAdmin_Admins.ContainsKey(CallerSteamID))
            {
                if (!string.IsNullOrEmpty(Localizer!["voteadmin.not.allowed"]))
                {
                    Helper.AdvancedPrintToChat(Caller, Localizer["voteadmin.not.allowed"]);
                }
                MenuManager.CloseActiveMenu(Caller);
                return HookResult.Continue;
            }

            var VoteGameModeMenu = new ChatMenu(Localizer!["voteadmin.choose.file"]);

            string[] jsonFiles = Directory.GetFiles(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/*.json");

            foreach (string jsonFile in jsonFiles)
            {
                string jsonData = File.ReadAllText(jsonFile);
                if (!string.IsNullOrEmpty(jsonData) && jsonData.Trim() != "[]" && jsonData.Trim() != "{}")
                {
                    string FileNames = Path.GetFileNameWithoutExtension(jsonFile);
                    VoteGameModeMenu.AddMenuOption(FileNames, (Caller, option) => HandleMenuALL(Caller, option));
                }
            }
            
            
            VoteGameModeMenu.AddMenuOption("Exit", SelectExit);
            MenuManager.OpenChatMenu(Caller, VoteGameModeMenu);
        }
        return HookResult.Continue;
    }
    private void HandleMenuALL(CCSPlayerController Caller, ChatMenuOption option)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string ChoosedFile = option.Text;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";
        string filePath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies", $"{ChoosedFile}.json");

        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var VoteGameModeMenu = new ChatMenu(Localizer!["voteadmin.choose.player"]);
        try
        {
            if (File.Exists(filePath))
            {
                
                string jsonData = File.ReadAllText(filePath);

                List<PlayerData> playerDataList = JsonConvert.DeserializeObject<List<PlayerData>>(jsonData)!;

                foreach (var playerData in playerDataList)
                {
                    VoteGameModeMenu.AddMenuOption(playerData.PlayerName!, (Caller, option) => HandleMenuALL2(Caller, option, filePath));
                }
            }
        }
        catch{}
        VoteGameModeMenu.AddMenuOption("Exit", SelectExit);
        MenuManager.OpenChatMenu(Caller, VoteGameModeMenu);
    }
    private void HandleMenuALL2(CCSPlayerController Caller, ChatMenuOption option, string folder)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string ChoosedPerson = option.Text;
        string Fpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/");
        string Time = DateTime.Now.ToString("HH:mm:ss");
        string Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        string Tpath = Path.Combine(cookiesFilePath,"../../plugins/Vote-GoldKingZ/logs/") + $"{fileName}";
        string filePath = folder;

        var CallerName = Caller.PlayerName;
        var CallerTeam = Caller.TeamNum;

        var VoteGameModeMenu = new ChatMenu(Localizer!["voteadmin.delete.player", ChoosedPerson]);
        try
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);

                List<PlayerData> playerDataList = JsonConvert.DeserializeObject<List<PlayerData>>(jsonData)!;
                int index = playerDataList.FindIndex(p => p.PlayerName == ChoosedPerson);
                string playerName = playerDataList[index].PlayerName!;
                ulong PlayerSteamID = playerDataList[index].PlayerSteamID;
                string PlayerIPAddress = playerDataList[index].PlayerIPAddress!;
                DateTime DateAndTime = playerDataList[index].DateAndTime!;
                int RestrictedForXDays = playerDataList[index].RestrictedForXDays;
                int RestrictedForXMins = playerDataList[index].RestrictedForXMins;
                string Reason = playerDataList[index].Reason!;

                Helper.AdvancedPrintToChat(Caller, Localizer!["voteadmin.player.detail"], playerName, PlayerSteamID, PlayerIPAddress, DateAndTime, RestrictedForXDays, RestrictedForXMins, Reason);
                
                string[] answers = { Localizer!["voteadmin.answer.yes"], Localizer!["voteadmin.answer.no"] };

                foreach (string answer in answers)
                {
                    VoteGameModeMenu.AddMenuOption(answer, (Caller, option) => HandleMenuALLAnswer(Caller, option, filePath, PlayerSteamID));
                }
            }
        }
        catch{}
        VoteGameModeMenu.AddMenuOption("Exit", SelectExit);
        MenuManager.OpenChatMenu(Caller, VoteGameModeMenu);
    }
    private void HandleMenuALLAnswer(CCSPlayerController Caller, ChatMenuOption option, string folder, ulong targetsteamid)
    {
        string filePath = folder;
        if (option.Text == Localizer!["voteadmin.answer.yes"])
        {

            try
            {
                string jsonData = File.ReadAllText(filePath);

                List<PlayerData> playerDataList = JsonConvert.DeserializeObject<List<PlayerData>>(jsonData)!;

                int index = playerDataList.FindIndex(p => p.PlayerSteamID == targetsteamid);
                string playerName = playerDataList[index].PlayerName!;
                if (index != -1)
                {
                    playerDataList.RemoveAt(index);

                    string updatedJsonData = JsonConvert.SerializeObject(playerDataList, Formatting.Indented);

                    File.WriteAllText(filePath, updatedJsonData);

                    Helper.AdvancedPrintToChat(Caller, Localizer!["voteadmin.player.successfully"], playerName);
                }
            }
            catch{}
        }else if (option.Text == Localizer!["voteadmin.answer.no"])
        {
           MenuManager.CloseActiveMenu(Caller); 
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
        var playerid = player.SteamID;
        
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;

        Globals_VoteAdmin.VoteAdmin_Admins.Remove(playerid);
        
        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariablesVoteAdmin();
    }
}