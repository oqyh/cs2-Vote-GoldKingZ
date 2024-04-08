# [CS2] Vote-GoldKingZ (1.0.7)

### Vote System (Kick , Mute , Banned, Vips)


![center](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/16a5904b-d618-4082-8678-ddbf7f42dce4)

![vk](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/45e3352d-7b9d-4d56-810e-df7efba9ca3d)

![kicked](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/1034a12f-91b2-4d67-8775-bf180c5d6839)


## Todo List

- [x] Vote Kick 
- [x] Vote Banned
- [x] Vote Mute (Voice)
- [ ] Vote Gag (Chat)
- [ ] Vote Silent (Chat + Voice)
- [ ] Vote Send Player Spec
- [ ] Vote A Map
- [ ] Vote A Mod (exec Config)

## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)




## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Vote-GoldKingZ\config\config.json                                           
>

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

  //Minimum Of Players To Start Vote Kick
  "VoteKick_StartOnMinimumOfXPlayers": 5,

  //Rest And Cancel VoteKick_TimeInMins On Map Change
  "VoteKick_AllowKickedPlayersToJoinOnMapChange": false,
  
  //VoteKick_TeamOnly (false) = Cross Teams Voting
  //VoteKick_TeamOnly (true) = Vote On Team Side Only
  "VoteKick_TeamOnly": false,
  
  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteKick_Percentage": 60,
  
  //If Vote Reach Half Depend Percentage On VoteKick_Percentage Do You Want Annoce Player To Vote shortcut Depend [VoteKick_CommandsOnHalfVoteAccept] And [VoteKick_CommandsOnHalfVoteRefuse] To Kick Player Announced
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

  //If You Put Any Group In The String Will Disable Vote Kick Once Join Game example:("@css/root,@css/admin")
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
  
  //If Vote Pass How Many In Days Should Banned Player Wait To Join Back
  "VoteBanned_TimeInDays": 5,

  //Minimum Of Players To Start Vote Ban
  "VoteBanned_StartOnMinimumOfXPlayers": 5,

  //VoteBanned_TeamOnly (false) = Cross Teams Voting
  //VoteBanned_TeamOnly (true) = Vote On Team Side Only
  "VoteBanned_TeamOnly": false,
  
  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteBanned_Percentage": 70,
  
  //If Vote Reach Half Depend Percentage On VoteBanned_Percentage Do You Want Annoce Player To Vote shortcut Depend [VoteBanned_CommandsOnHalfVoteAccept] And [VoteBanned_CommandsOnHalfVoteRefuse] To Banned Player Announced
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

  //If You Put Any Group In The String Will Disable Vote Ban Once Join Game example:("@css/root,@css/admin")
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

  //Minimum Of Players To Start Vote Mute
  "VoteMute_StartOnMinimumOfXPlayers": 5,

  //Rest And Cancel VoteMute_TimeInMins On Map Change
  "VoteMute_RemoveMutedPlayersOnMapChange": false,

  //VoteMute_TeamOnly (false) = Cross Teams Voting
  //VoteMute_TeamOnly (true) = Vote On Team Side Only
  "VoteMute_TeamOnly": false,

  //Vote Percentage To Pass The Vote 60 means 60% || 50 means 50% Half
  "VoteMute_Percentage": 60,

  //If Vote Reach Half Depend Percentage On VoteMute_Percentage Do You Want Annoce Player To Vote shortcut Depend [VoteMute_CommandsOnHalfVoteAccept] And [VoteMute_CommandsOnHalfVoteRefuse] To Mute Player Announced
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

  //If You Put Any Group In The String Will Disable Vote Mute Once Join Game example:("@css/root,@css/admin")
  "VoteMute_DisableItOnJoinTheseGroups": "",
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
    //{STEAMID} = ex: 76561198206086993
    //{IP} = ex: 127.0.0.0
    //==========================

    //Enable Or Disable Log Local
    "Log_SendLogToText": false,

    //If Log_SendLogToText Enabled How Do You Like Message Look Like
    "Log_TextMessageFormat": "[{DATE} - {TIME}] {PLAYERNAME} Has Been ({REASON})  [SteamID: {STEAMID} - Ip: {IP}]",

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
    "votekick.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='grey'>Kick player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Kick Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Remove Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",


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
    "votebanned.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Ban player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Ban Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Remove Ban Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",


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
    "votemute.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Mute player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Mute Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Remove Mute Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>"
}
```

## .:[ Change Log ]:.
```
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
