# [CS2] Vote-GoldKingZ (1.0.2)

### Vote System (Kick , Mute , Banned, Vips)


![center](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/16a5904b-d618-4082-8678-ddbf7f42dce4)

![vk](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/45e3352d-7b9d-4d56-810e-df7efba9ca3d)

![kicked](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/1034a12f-91b2-4d67-8775-bf180c5d6839)


## Todo List

- [x] Vote Kick 
- [x] Vote Banned
- [ ] Vote Mute (Chat)
- [ ] Vote Gag (Voice)
- [ ] Vote Silent (Chat + Voice)
- [ ] Vote Send Player Spec
- [ ] Vote A Map

## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)




## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Vote-GoldKingZ\config\config.json                                           
> After Upload Plugin Check Server Console For 100% Loaded Message And Not Facing Any Errors                                          
>                                                                                      
> ![loaded](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/18b78f36-7129-494c-8e0d-655609d3bd06)

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

  //Rest And Cancel AfterKickGivePlayerXMinsFromJoining On Map Change
  "VoteKick_AllowKickedPlayersToJoinOnMapChange": false,
  
  //VoteKick_TeamOnly (false) = Cross Teams Voting
  //VoteKick_TeamOnly (true) = Vote On Team Side Only
  "VoteKick_TeamOnly": false,
  
  //Vote Percentage To Pass The Vote 6 means 0.6 || 5 means 0.5 Half
  "VoteKick_Percentage": 6,
  
  //If Vote Reach Half Depend Percentage On VoteKick_Percentage Do You Want Annoce Player To Vote shortcut Depend [VoteKick_CommandsOnHalfVoteAccept] And [VoteKick_CommandsOnHalfVoteRefuse] To Kick Player Announced
  "VoteKick_CenterMessageAnnouncementOnHalfVotes": true,
  
  //If VoteKick_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteKick_CenterMessageAnnouncementTimer": 25,
  
  //Enable Punishment Only Who Try To Evasion VoteKick_Mode Only Works 2 to 4
  "VoteKick_EvasionPunishment": false,
  
  //If VoteKick_EvasionPunishment Enabled How Many In Mins Give Extra For Evasion Punishment
  "VoteKick_EvasionPunishmentTimeInMins": 10,

  //Commands Ingame
  "VoteKick_CommandsToVote": "!votekick,!kick,!vk",
  "VoteKick_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteKick_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Immunity From Getting Vote To Kick
  "VoteKick_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
  
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
  
  //Vote Percentage To Pass The Vote 6 means 0.6 || 5 means 0.5 Half
  "VoteBanned_Percentage": 6,
  
  //If Vote Reach Half Depend Percentage On VoteBanned_Percentage Do You Want Annoce Player To Vote shortcut Depend [VoteBanned_CommandsOnHalfVoteAccept] And [VoteBanned_CommandsOnHalfVoteRefuse] To Banned Player Announced
  "VoteBanned_CenterMessageAnnouncementOnHalfVotes": true,
  
  //If VoteBanned_CenterMessageAnnouncementOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteBanned_CenterMessageAnnouncementTimer": 25,
  
  //Enable Punishment Only Who Try To Evasion VoteBanned_Mode Only Works 2 to 4
  "VoteBanned_EvasionPunishment": false,
  
  //If VoteBanned_EvasionPunishment Enabled How Many In Days Give Extra For Evasion Punishment
  "VoteBanned_EvasionPunishmentTimeInDays": 10,

  //Commands Ingame
  "VoteBanned_CommandsToVote": "!votebanned,!banned,!vb",
  "VoteBanned_CommandsOnHalfVoteAccept": "!yes,yes,!y,y",
  "VoteBanned_CommandsOnHalfVoteRefuse": "!no,no,!n,n",

  //Immunity From Getting Vote To Banned
  "VoteBanned_ImmunityGroups": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
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

    "votekick.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Kick You Need Minimum {lime}{0} {grey}Players",    //{0} Players Needed
    "votekick.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Kick VIPs",    //{0} Vip PlayerName 

    "votekick.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Kick {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",    //{0} PlayerName Vote On - {1} Votes - {2} Needed
    "votekick.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",    //{0} PlayerName Vote On - {1} Votes - {2} Needed
    "votekick.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",    //{0} PlayerName Vote On - {1} Votes - {2} Needed

    "votekick.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Kick {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",    //{0} PlayerName Rock The Vote - {1} PlayerName Vote On - {1} Votes - {2} Needed

    "votekick.announce.kick.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Kicked",    //{0} PlayerName Kicked
    "votekick.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Kick",

    "votekick.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='grey'>Kick player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Kick Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Remove Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>",    // {0} Timer - {1} PlayerName Vote On - {2} Votes - {3} Needed


    



    

    "votebanned.menu.name": "{purple}Vote Banned Menu",

    "votebanned.minimum.needed": "{green}Gold KingZ {grey}| {grey}You Cant Start Vote Banned You Need Minimum {lime}{0} {grey}Players",    //{0} Players Needed
    "votebanned.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Banned VIPs",    //{0} Vip PlayerName 

    "votebanned.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Banned {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",    //{0} PlayerName Vote On - {1} Votes - {2} Needed
    "votebanned.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",    //{0} PlayerName Vote On - {1} Votes - {2} Needed
    "votebanned.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]",    //{0} PlayerName Vote On - {1} Votes - {2} Needed

    "votebanned.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted To Banned {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]",    //{0} PlayerName Rock The Vote - {1} PlayerName Vote On - {1} Votes - {2} Needed

    "votebanned.announce.banned.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Banned",    //{0} PlayerName Banned
    "votebanned.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Banned",

    "votebanned.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='red'>Banned player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Banned Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Remove Banned Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>"    // {0} Timer - {1} PlayerName Vote On - {2} Votes - {3} Needed

}
```

## .:[ Change Log ]:.
```
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
