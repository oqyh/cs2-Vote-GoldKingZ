# [CS2] Vote-GoldKingZ (1.0.9)

### Vote (Kick, Banned, Mute, Gag, Silent, Gamemode, Map, Vips)


![center](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/16a5904b-d618-4082-8678-ddbf7f42dce4)

![vk](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/45e3352d-7b9d-4d56-810e-df7efba9ca3d)

![kicked](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/1034a12f-91b2-4d67-8775-bf180c5d6839)


## Todo List

- [x] Admin Control Votes InGame
- [x] Vote Kick 
- [x] Vote Banned
- [x] Vote Mute (Voice)
- [x] Vote Gag (Chat)
- [x] Vote Silent (Chat + Voice)
- [x] Vote A Game Mod (exec Config)
- [x] Vote A Map
- [ ] Vote Send Player Spec



## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)




## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Vote-GoldKingZ\config\config.json                                           
>

<p><details><summary> [ Vote Admin ] </summary>

```json
{
  //Commands Ingame To Open Voted Menu
  "VoteAdmin_CommandsInGame": "!votesadmin,!voteadmin",

  //Admin Flags
  "VoteAdmin_Groups": "@css/root,@css/admin",
}
```

</details>
</p>

<p><details><summary> [ Vote Kick ] </summary>

```json
{
  //After Kicking Player Which Method Do You Like
  //VoteKick_Mode (0) = Disable
  //VoteKick_Mode (1) = Kick Only
  //VoteKick_Mode (2) = Kick And Restrict SteamID From Joining
  //VoteKick_Mode (3) = Kick And Restrict IpAddress From Joining
  //VoteKick_Mode (4) = Kick And Restrict SteamID And IpAddress From Joining
  "VoteKick_Mode": 2,
  
  //If Vote Pass How Many In Mins Should Kicked Player Wait To Join Back
  "VoteKick_TimeInMins": 5,

  //Change VoteKick_TimeInMins To Days
  "VoteKick_ChangeTimeInMinsToDays": false,

  //Minimum Of Players To Start Vote Kick
  "VoteKick_StartOnMinimumOfXPlayers": 5,

  //Rest And Cancel VoteKick_TimeInMins On Map Change
  "VoteKick_AllowKickedPlayersToJoinOnMapChange": false,
  
  //VoteKick_TeamOnly (false) = Cross Teams Voting
  //VoteKick_TeamOnly (true) = Vote On Team Side Only
  "VoteKick_TeamOnly": false,
  
  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteKick_Percentage": 60,
  
  //If Vote Reach Half Depend Percentage On VoteKick_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteKick_CommandsOnHalfVoteAccept] And [VoteKick_CommandsOnHalfVoteRefuse] To Kick Player Announced
  "VoteKick_CenterMessageAnnouncementOnHalfVotes": true,
  
  //If VoteKick_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteKick_CenterMessageAnnouncementTimer": 25,
  
  //Enable Punishment Only Who Try To Evasion VoteKick_Mode Only Works 2 to 4 With New Accounts
  "VoteKick_EvasionPunishment": false,
  
  //If VoteKick_EvasionPunishment Enabled How Many In Mins Give Extra For Evasion Punishment
  "VoteKick_EvasionPunishmentTimeInMins": 10,

  //Delay Kick To Show Message (votekick.player.delay.message) Then Kick
  "VoteKick_DelayKick": false,

  //Commands Ingame
  "VoteKick_CommandsToVote": "!votekick,!kick,!vk",
  "VoteKick_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteKick_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Immunity From Getting Vote To Kick
  "VoteKick_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",

  //If You Put Any Group In The String Will Disable Vote Kick Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteKick_DisableItOnJoinTheseGroups": "",
}
```

</details>
</p>


<p><details><summary> [ Vote Banned ] </summary>

```json
{
  //After Banned Player Which Method Do You Like
  //VoteBanned_Mode (0) = Disable
  //VoteBanned_Mode (1) = Banned And Restrict SteamID From Joining
  //VoteBanned_Mode (2) = Banned And Restrict IpAddress From Joining
  //VoteBanned_Mode (3) = Banned And Restrict SteamID And IpAddress From Joining
  "VoteBanned_Mode": 0,
  
  //If Vote Pass How Many In Mins Should Banned Player Wait To Join Back
  "VoteBanned_TimeInMins": 1,

  //Change VoteBanned_TimeInMins To Days
  "VoteBanned_ChangeTimeInMinsToDays": true,

  //Minimum Of Players To Start Vote Ban
  "VoteBanned_StartOnMinimumOfXPlayers": 5,

  //VoteBanned_TeamOnly (false) = Cross Teams Voting
  //VoteBanned_TeamOnly (true) = Vote On Team Side Only
  "VoteBanned_TeamOnly": false,
  
  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteBanned_Percentage": 70,
  
  //If Vote Reach Half Depend Percentage On VoteBanned_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteBanned_CommandsOnHalfVoteAccept] And [VoteBanned_CommandsOnHalfVoteRefuse] To Banned Player Announced
  "VoteBanned_CenterMessageAnnouncementOnHalfVotes": true,
  
  //If VoteBanned_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteBanned_CenterMessageAnnouncementTimer": 25,
  
  //Enable Punishment Only Who Try To Evasion VoteBanned_Mode Only Works 2 to 4 With New Accounts
  "VoteBanned_EvasionPunishment": false,
  
  //If VoteBanned_EvasionPunishment Enabled How Many In Days Give Extra For Evasion Punishment
  "VoteBanned_EvasionPunishmentTimeInDays": 10,

  //Delay Kick To Show Message (votebanned.player.delay.message) Then Kick
  "VoteBanned_DelayKick": false,

  //Commands Ingame
  "VoteBanned_CommandsToVote": "!votebanned,!banned,!vb",
  "VoteBanned_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteBanned_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Immunity From Getting Vote To Banned
  "VoteBanned_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",

  //If You Put Any Group In The String Will Disable Vote Ban Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteBanned_DisableItOnJoinTheseGroups": "",
}
```

</details>
</p>


<p><details><summary> [ Vote Mute ] </summary>

```json
{
  //Enable Or Disable Vote Mute
  "VoteMute": false,

  //If Vote Pass How Many In Mins Should Mute Player
  "VoteMute_TimeInMins": 5,

  //Change VoteMute_TimeInMins To Days
  "VoteMute_ChangeTimeInMinsToDays": false,

  //Minimum Of Players To Start Vote Mute
  "VoteMute_StartOnMinimumOfXPlayers": 5,

  //Rest And Cancel VoteMute_TimeInMins On Map Change
  "VoteMute_RemoveMutedPlayersOnMapChange": false,

  //VoteMute_TeamOnly (false) = Cross Teams Voting
  //VoteMute_TeamOnly (true) = Vote On Team Side Only
  "VoteMute_TeamOnly": false,

  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteMute_Percentage": 60,

  //If Vote Reach Half Depend Percentage On VoteMute_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteMute_CommandsOnHalfVoteAccept] And [VoteMute_CommandsOnHalfVoteRefuse] To Mute Player Announced
  "VoteMute_CenterMessageAnnouncementOnHalfVotes": false,

  //If VoteMute_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteMute_CenterMessageAnnouncementTimer": 25,

  //Enable Punishment Only Who Try To Evasion VoteMute With New Accounts
  "VoteMute_EvasionPunishment": false,

  //If VoteMute_EvasionPunishment Enabled How Many In Mins Give Extra For Evasion Punishment
  "VoteMute_EvasionPunishmentTimeInMins": 10,

  //Commands Ingame
  "VoteMute_CommandsToVote": "!votemute,!mute,!vm",
  "VoteMute_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteMute_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Immunity From Getting Vote To Mute
  "VoteMute_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",

  //If You Put Any Group In The String Will Disable Vote Mute Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteMute_DisableItOnJoinTheseGroups": "",
}
```

</details>
</p>


<p><details><summary> [ Vote Gag ] </summary>

```json
{
  //Enable Or Disable Vote Gag
  "VoteGag": false,
  
  //If Vote Pass How Many In Mins Should Gag Player
  "VoteGag_TimeInMins": 5,

  //Change VoteGag_TimeInMins To Days
  "VoteGag_ChangeTimeInMinsToDays": false,

  //Minimum Of Players To Start Vote Gag
  "VoteGag_StartOnMinimumOfXPlayers": 5,

  //Rest And Cancel VoteGag_TimeInMins On Map Change
  "VoteGag_RemoveGagedPlayersOnMapChange": false,

  //VoteGag_TeamOnly (false) = Cross Teams Voting
  //VoteGag_TeamOnly (true) = Vote On Team Side Only
  "VoteGag_TeamOnly": false,

  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteGag_Percentage": 60,

  //If Vote Reach Half Depend Percentage On VoteGag_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteGag_CommandsOnHalfVoteAccept] And [VoteGag_CommandsOnHalfVoteRefuse] To Gag Player Announced
  "VoteGag_CenterMessageAnnouncementOnHalfVotes": false,

  //If VoteGag_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteGag_CenterMessageAnnouncementTimer": 25,

  //Enable Punishment Only Who Try To Evasion VoteGag With New Accounts
  "VoteGag_EvasionPunishment": false,

  //If VoteGag_EvasionPunishment Enabled How Many In Mins Give Extra For Evasion Punishment
  "VoteGag_EvasionPunishmentTimeInMins": 10,

  //Commands Ingame
  "VoteGag_CommandsToVote": "!votegag,!gag,!vg",
  "VoteGag_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteGag_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Allow Gaged Player To Use These !.@ (OtherWise Gaged Player Cannot Exec Any Commands To Other Plugins like !rtv)
  "VoteGag_LetTheseAllowedForGagedPlayers": "!,.,@",

  //Immunity From Getting Vote To Mute
  "VoteGag_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",

  //If You Put Any Group In The String Will Disable Vote Gag Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteGag_DisableItOnJoinTheseGroups": "",
}
```

</details>
</p>

<p><details><summary> [ Vote Silent (Chat + Voice) ] </summary>

```json
{
  //Enable Or Disable Vote Silent
  "VoteSilent": false,
  
  //If Vote Pass How Many In Mins Should Silent Player
  "VoteSilent_TimeInMins": 5,

  //Change VoteSilent_TimeInMins To Days
  "VoteSilent_ChangeTimeInMinsToDays": false,

  //Minimum Of Players To Start Vote Silent
  "VoteSilent_StartOnMinimumOfXPlayers": 5,

  //Rest And Cancel VoteSilent_TimeInMins On Map Change
  "VoteSilent_RemoveSilentedPlayersOnMapChange": false,

  //VoteSilent_TeamOnly (false) = Cross Teams Voting
  //VoteSilent_TeamOnly (true) = Vote On Team Side Only
  "VoteSilent_TeamOnly": false,

  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteSilent_Percentage": 60,

  //If Vote Reach Half Depend Percentage On VoteSilent_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteSilent_CommandsOnHalfVoteAccept] And [VoteSilent_CommandsOnHalfVoteRefuse] To Silent Player Announced
  "VoteSilent_CenterMessageAnnouncementOnHalfVotes": false,

  //If VoteSilent_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteSilent_CenterMessageAnnouncementTimer": 25,

  //Enable Punishment Only Who Try To Evasion VoteSilent With New Accounts
  "VoteSilent_EvasionPunishment": false,

  //If VoteSilent_EvasionPunishment Enabled How Many In Mins Give Extra For Evasion Punishment
  "VoteSilent_EvasionPunishmentTimeInMins": 10,

  //Commands Ingame
  "VoteSilent_CommandsToVote": "!votesilent,!slt,!vs",
  "VoteSilent_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteSilent_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Allow Silented Player To Use These !.@ (OtherWise Silented Player Cannot Exec Any Commands To Other Plugins like !rtv)
  "VoteSilent_LetTheseAllowedForSilentedPlayers": "!,.,@",

  //Immunity From Getting Vote To Mute
  "VoteSilent_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",

  //If You Put Any Group In The String Will Disable Vote Silent Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteSilent_DisableItOnJoinTheseGroups": "",
}
```
</details>
</p>

<details>
<summary> [ Vote Game Mode ] </summary>

> Note: Config Located In ..\addons\counterstrikesharp\plugins\Vote-GoldKingZ\config\VoteGameMode.json                                          
> After Setup Game Modes Names Put Your Cfgs In Folder Located In ..\cfg\Vote-GoldKingZ\

```json
{
  //Enable Or Disable Vote Game Mode
  "VoteGameMode": false,

  //Minimum Of Players To Start Vote Game Mode
  "VoteGameMode_StartOnMinimumOfXPlayers": 5,

  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteGameMode_Percentage": 60,

  //If Vote Reach Half Depend Percentage On VoteGameMode_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteGameMode_CommandsOnHalfVoteAccept] And [VoteGameMode_CommandsOnHalfVoteRefuse] To Game Mode Announced
  "VoteGameMode_CenterMessageAnnouncementOnHalfVotes": false,

  //If VoteSilent_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteGameMode_CenterMessageAnnouncementTimer": 25,

  //Commands Ingame
  "VoteGameMode_CommandsToVote": "!votemode,!votem",
  "VoteGameMode_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteGameMode_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //If You Put Any Group In The String Will Disable Vote Game Mode Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteGameMode_DisableItOnJoinTheseGroups": "",
}
```

</details>
</p>

<details>
<summary> [ Vote Map ] </summary>

> Note: Config Located In ..\addons\counterstrikesharp\plugins\Vote-GoldKingZ\config\VoteMap.json                                          
> After Setup Maps You Can Use `"Display": "New Name"` To Rename Map Name In Menu                                                                                    
> `host:` Means Link Of The Map Example https://steamcommunity.com/sharedfiles/filedetails/?id=3084197740 Will Be `host:3084197740`                                          
> `ds:` Means What What Server Map Has In `ds_workshop_listmaps`                                                                                    
> `None Prefix` Means Normal Map de_dust2, cs_office etc..                                                                                                                             

```json
{
  //Enable Or Disable Vote Map
  "VoteMap": false,

  //Minimum Of Players To Start Vote Map
  "VoteMap_StartOnMinimumOfXPlayers": 5,

  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteMap_Percentage": 60,

  //If Vote Reach Half Depend Percentage On VoteMap_Percentage Do You Want Annoce Players To Vote shortcut Depend [VoteMap_CommandsOnHalfVoteAccept] And [VoteMap_CommandsOnHalfVoteRefuse] To Map Announced
  "VoteMap_CenterMessageAnnouncementOnHalfVotes": false,

  //If VoteSilent_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteMap_CenterMessageAnnouncementTimer": 25,

  //Commands Ingame
  "VoteMap_CommandsToVote": "!votemaps,!votemap",
  "VoteMap_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteMap_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //If You Put Any Group In The String Will Disable Vote Game Mode Once Join Game example:("@css/root,@css/admin,#css/admin")
  "VoteMap_DisableItOnJoinTheseGroups": "",
}
```

</details>
</p>

<p><details><summary> [ Log ] </summary>

```json
{
    //==========================
    //        Format
    //==========================
    //{DATE} = Date DD-MM-YYYY
    //{TIME} = Time HH:mm:ss
    //{PLAYERNAME} = Player Name
    //{REASON} = Reason (votekick,vote banned,etc...)
    //{GAMEMODE} = Game Mode Name
    //{MAP} = Map Name
    //{STEAMID} = ex: 76561198206086993
    //{IP} = ex: 127.0.0.0
    //==========================

    //Enable Or Disable Log Local
    "Log_SendLogToText": false,

    //If Log_SendLogToText Enabled How Do You Like Message Look Like
    "Log_TextMessageFormat": "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]",
    "Log_GameModeFormat": "[{DATE} - {TIME}] Vote Game Mode Choosed To Change To ({GAMEMODE})",
    "Log_MapFormat": "[{DATE} - {TIME}] Vote Map Choosed To Change To ({MAP})",

    //If Log_SendLogToText Enabled Auto Delete Logs If More Than X (Days) Old
    "Log_AutoDeleteLogsMoreThanXdaysOld": 0,

    //Send Log To Discord Via WebHookURL
    //Log_SendLogToDiscordOnMode (0) = Disable
    //Log_SendLogToDiscordOnMode (1) = Text Only
    //Log_SendLogToDiscordOnMode (2) = Text With + Name + Hyperlink To Steam Profile
    //Log_SendLogToDiscordOnMode (3) = Text With + Name + Hyperlink To Steam Profile + Profile Picture
    "Log_SendLogToDiscordOnMode": 0,

    //If Log_SendLogToDiscordOnMode (2) or Log_SendLogToDiscordOnMode (3) How Would You Side Color Message To Be Check (https://www.color-hex.com/) For Colors
    "Log_DiscordSideColor": "00FFFF",

    //Discord WebHook
    "Log_DiscordWebHookURL": "https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",

    //If Log_SendLogToDiscordOnMode (1) or (2) or (3) How Do You Like Message Look Like
    "Log_DiscordMessageFormat": "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]",
    "Log_DiscordGameModeFormat": "[{DATE} - {TIME}] Vote Game Mode Choosed To Change To ({GAMEMODE})",
    "Log_DiscordMapFormat": "[{DATE} - {TIME}] Vote Map Choosed To Change To ({MAP})",

    //If Log_SendLogToDiscordOnMode (3) And Player Doesn't Have Profile Picture Which Picture Do You Like To Be Replaced
    "Log_DiscordUsersWithNoAvatarImage": "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/b5/b5bd56c1aa4644a474a2e4972be27ef9e82e517e_full.jpg",
}
```

</details>
</p>


## .:[ Language ]:.

![colors](https://github.com/oqyh/cs2-vote-kick/assets/48490385/617503c9-fe77-480d-9ce2-fca5299cdcd5)


```json

{
	//==========================
    //        Colors
    //==========================
    //{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
    //{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
    //{Default} {White} {Darkred} {Green} {LightYellow}
    //==========================
    //        Other
    //==========================
    //{nextline} = Print On Next Line
	//<br> = Next Line On HUD Center Message
    //==========================
	
    "voteadmin.not.allowed": "{green}Gold KingZ {grey}| {darkred}This Command Only For Admins Only",
    "voteadmin.choose.file": "{purple}Choose Which File",
    "voteadmin.choose.player": "{purple}Choose Which Player",
    "voteadmin.delete.player": "{purple}Are You Sure Want To Delete {lime} {0} {grey}?",
    "voteadmin.player.detail": "{darkred}-------------------------------------------- {nextline} {green}Gold KingZ {grey}| Name: {Purple}{0} {grey}|| SteamID: {Purple}{1}{nextline}{green}Gold KingZ {grey}| IpAdress: {Purple}{2}{nextline}{green}Gold KingZ {grey}| Date: {Purple}{3}{nextline}{green}Gold KingZ {grey}| Restricted: {grey}Days:{Purple}{4} {grey}Mins:{Purple}{5}{nextline}{green}Gold KingZ {grey}| Reason: {lime}{6}{nextline}{darkred}-------------------------------------------- ",
    "voteadmin.answer.yes": "{lime}Yes",
    "voteadmin.answer.no": "{darkred}No",
    "voteadmin.player.successfully": "{green}Gold KingZ {grey}| {Purple}{0} {grey}successfully Deleted",

    "votekick.menu.name": "{purple}Vote Kick Menu",
    "votekick.player.is.disabled": "{green}Gold KingZ {grey}| Vote Kick Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votekick.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Kick You Need Minimum {lime}{0} {grey}Players",
    "votekick.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Kick VIPs",
    "votekick.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Kick {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votekick.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votekick.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votekick.player.delay.message": "{green}Gold KingZ {grey}| You Will Be Kicked Need To Wait {red}{0} Mins",
    "votekick.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Kick {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votekick.announce.kick.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Kicked",
    "votekick.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Kick",
    "votekick.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='grey'>Kick player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",


    "votebanned.menu.name": "{purple}Vote Ban Menu",
    "votebanned.player.is.disabled": "{green}Gold KingZ {grey}| Vote Ban Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votebanned.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Ban You Need Minimum {lime}{0} {grey}Players",
    "votebanned.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Ban VIPs",
    "votebanned.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Ban {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votebanned.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votebanned.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votebanned.player.delay.message": "{green}Gold KingZ {grey}| You Are Banned Need To Wait {red}{0} Days",
    "votebanned.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Ban {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votebanned.announce.banned.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Banned",
    "votebanned.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Ban",
    "votebanned.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Ban player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Ban Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Ban Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",


    "votemute.menu.name": "{purple}Vote Mute Menu",
    "votemute.player.is.disabled": "{green}Gold KingZ {grey}| Vote Mute Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votemute.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Mute You Need Minimum {lime}{0} {grey}Players",
    "votemute.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Mute VIPs",
    "votemute.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Mute {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votemute.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votemute.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votemute.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Mute {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votemute.player.muted.successfully.message": "{green}Gold KingZ {grey}| You Are Muted For {red}{0} {grey}Mins",
    "votemute.announce.muted.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Muted",
    "votemute.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Mute",
    "votemute.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Mute player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Mute Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Mute Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",
    
    "votegag.menu.name": "{purple}Vote Gag Menu",
    "votegag.player.is.disabled": "{green}Gold KingZ {grey}| Vote Gag Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votegag.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Gag You Need Minimum {lime}{0} {grey}Players",
    "votegag.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Gag VIPs",
    "votegag.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Gag {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votegag.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votegag.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votegag.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Gag {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votegag.player.gaged.successfully.message": "{green}Gold KingZ {grey}| You Are Gaged For {red}{0} {grey}Mins",
    "votegag.announce.gaged.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Gaged",
    "votegag.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Gag",
    "votegag.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Gag player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Gag Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Gag Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",

    "votesilent.menu.name": "{purple}Vote Silent Menu",
    "votesilent.player.is.disabled": "{green}Gold KingZ {grey}| Vote Silent Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votesilent.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Silent You Need Minimum {lime}{0} {grey}Players",
    "votesilent.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Silent VIPs",
    "votesilent.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Silent {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votesilent.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votesilent.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votesilent.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Silent {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votesilent.player.silented.successfully.message": "{green}Gold KingZ {grey}| You Are Silented For {red}{0} {grey}Mins",
    "votesilent.announce.silented.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Silented",
    "votesilent.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Silent",
    "votesilent.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Silent player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Silent Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Silent Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",

    "votegamemode.menu.name": "{purple}Vote Game Mode Menu",
    "votegamemode.player.is.disabled": "{green}Gold KingZ {grey}| Vote Game Mode Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votegamemode.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Game Mode You Need Minimum {lime}{0} {grey}Players",
    "votegamemode.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Game Mode {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votegamemode.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votegamemode.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votegamemode.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Change Game Mode To {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votegamemode.announce.gamemode.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, Changing Game Mode To {Purple}{0}",
    "votegamemode.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Game Mode",
    "votegamemode.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Change Mode: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Vote Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Vote Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",
    
    "votemap.menu.name": "{purple}Vote Map Menu",
    "votemap.player.is.disabled": "{green}Gold KingZ {grey}| Vote Map Is Temporarily {darkred}Disabled {grey}Admin In The Game",
    "votemap.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Map You Need Minimum {lime}{0} {grey}Players",
    "votemap.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Map {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votemap.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votemap.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",
    "votemap.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Change Map To {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",
    "votemap.announce.map.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, Changing Map To {Purple}{0}",
    "votemap.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Map",
    "votemap.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Change Map: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Accept Vote Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Decline Vote Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>"
}

```

## .:[ Change Log ]:.
```
(1.0.9)
-Fix Some Bugs

  - [ Vote Admin ]
Added VoteAdmin_CommandsInGame
Added VoteAdmin_Groups
Lang VoteAdmin

  - [ Vote Map ]
Added VoteMap
Added VoteMap_StartOnMinimumOfXPlayers
Added VoteMap_Percentage
Added VoteMap_CenterMessageAnnouncementOnHalfVotes
Added VoteMap_CenterMessageAnnouncementTimer
Added VoteMap_CommandsToVote
Added VoteMap_CommandsOnHalfVoteAccept
Added VoteMap_CommandsOnHalfVoteRefuse
Added VoteMap_DisableItOnJoinTheseGroups 
Lang VoteMap

  - [ Vote Banned ]
Fix Some Bugs
Fix Null IpAdress
Change VoteBanned_TimeInDays To VoteBanned_TimeInMins
Added VoteBanned_ChangeTimeInMinsToDays

  - [ Vote Gag ]
Fix Some Bugs
Fix Null IpAdress
Added VoteGag_ChangeTimeInMinsToDays

  - [ Vote Kick ]
Fix Some Bugs
Fix Null IpAdress
Added VoteKick_ChangeTimeInMinsToDays



  - [ Vote Mute ]
Fix Some Bugs
Fix Null IpAdress
Added VoteMute_ChangeTimeInMinsToDays

  - [ Vote Silent ]
Fix Some Bugs
Fix Null IpAdress
Added VoteSilent_ChangeTimeInMinsToDays

  - [ Log ]
Added Log_MapFormat
Added Log_DiscordMapFormat

(1.0.8)
-Fix Some Bugs
-Remove Bot From Counting + List Players

  - [ Vote Gag ]
Added VoteGag
Added VoteGag_TimeInMins
Added VoteGag_StartOnMinimumOfXPlayers
Added VoteGag_RemoveGagedPlayersOnMapChange
Added VoteGag_TeamOnly
Added VoteGag_Percentage
Added VoteGag_CenterMessageAnnouncementOnHalfVotes
Added VoteGag_CenterMessageAnnouncementTimer
Added VoteGag_EvasionPunishment
Added VoteGag_EvasionPunishmentTimeInMins
Added VoteGag_CommandsToVote
Added VoteGag_CommandsOnHalfVoteAccept
Added VoteGag_CommandsOnHalfVoteRefuse
Added VoteGag_LetTheseAllowedForGagedPlayers
Added VoteGag_ImmunityGroups
Added VoteGag_DisableItOnJoinTheseGroups
Lang VoteGag

  - [ Vote Silent ]
Added VoteSilent
Added VoteSilent_TimeInMins
Added VoteSilent_StartOnMinimumOfXPlayers
Added VoteSilent_RemoveSilentedPlayersOnMapChange
Added VoteSilent_TeamOnly
Added VoteSilent_Percentage
Added VoteSilent_CenterMessageAnnouncementOnHalfVotes
Added VoteSilent_CenterMessageAnnouncementTimer
Added VoteSilent_EvasionPunishment
Added VoteSilent_EvasionPunishmentTimeInMins
Added VoteSilent_CommandsToVote
Added VoteSilent_CommandsOnHalfVoteAccept
Added VoteSilent_CommandsOnHalfVoteRefuse
Added VoteSilent_LetTheseAllowedForSilentedPlayers
Added VoteSilent_ImmunityGroups
Added VoteSilent_DisableItOnJoinTheseGroups
Lang VoteSilent

  - [ Vote Game Mode ]
Added VoteGameMode
Added VoteGameMode_StartOnMinimumOfXPlayers
Added VoteGameMode_Percentage
Added VoteGameMode_CenterMessageAnnouncementOnHalfVotes
Added VoteGameMode_CenterMessageAnnouncementTimer
Added VoteGameMode_CommandsToVote
Added VoteGameMode_CommandsOnHalfVoteAccept
Added VoteGameMode_CommandsOnHalfVoteRefuse
Added VoteGameMode_DisableItOnJoinTheseGroups
Lang VoteGameMode

- [ Log ]
Fix Writing Bug After Creating Files Log On (Vote Banned,Vote Kick,Vote Mute)
Fix Log_DiscordMessageFormat On (Vote Banned,Vote Kick,Vote Mute)
Added Log_GameModeFormat
Added Log_DiscordGameModeFormat

(1.0.7)
-Fix Counting And Name List

(1.0.6)
-Changing VoteKick_Percentage to float
-Changing VoteBanned_Percentage to float
-Changing VoteMute_Percentage to float

(1.0.5)
-Added VoteKick_DisableItOnJoinTheseGroups
-Added VoteBanned_DisableItOnJoinTheseGroups
-Added VoteMute_DisableItOnJoinTheseGroups
-Added Lang votekick.player.is.disabled
-Added Lang votebanned.player.is.disabled
-Added Lang votemute.player.is.disabled

(1.0.4)
-Fix Some Bugs
-Fix "votekick.announce.halfvotes.center.message" instead "votebanned.announce.halfvotes.center.message"

(1.0.3)
  - [ Vote Kick ]
Fix Bugs Vote Kick
Added VoteKick_DelayKick
Lang "votekick.player.delay.message"
 
  - [ Vote Ban ]
Fix Bugs Vote Ban
Added VoteBanned_DelayKick
Lang "votebanned.player.delay.message"
 
  - [ Vote Mute ]
Added VoteMute 
Added VoteMute_TimeInMins 
Added VoteMute_StartOnMinimumOfXPlayers  
Added VoteMute_RemoveMutedPlayersOnMapChange 
Added VoteMute_TeamOnly 
Added VoteMute_Percentage 
Added VoteMute_CenterMessageAnnouncementOnHalfVotes 
Added VoteMute_CenterMessageAnnouncementTimer 
Added VoteMute_EvasionPunishment 
Added VoteMute_EvasionPunishmentTimeInMins 
Added VoteMute_CommandsToVote 
Added VoteMute_CommandsOnHalfVoteAccept 
Added VoteMute_CommandsOnHalfVoteRefuse 
Added VoteMute_ImmunityGroups
Lang VoteMute

  - [ Logs ]
  
Added Log_SendLogToText 
Added Log_TextMessageFormat 
Added Log_AutoDeleteLogsMoreThanXdaysOld 
Added Log_SendLogToDiscordOnMode 
Added Log_DiscordSideColor 
Added Log_DiscordWebHookURL 
Added Log_DiscordMessageFormat 
Added Log_DiscordUsersWithNoAvatarImage

(1.0.2)
-Fix Cross Vote Announcement Banned, Kick

(1.0.1)
-Added "VoteKick_StartOnMinimumOfXPlayers" In Json To Vote Kick
-Added "votekick.minimum.needed" In Lang For Vote Kick
-Added Vote Banned

(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
