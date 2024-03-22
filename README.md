# [CS2] Vote-GoldKingZ (1.0.0)

### Vote System (Kick , Mute , Banned, Vips)


![center](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/16a5904b-d618-4082-8678-ddbf7f42dce4)

![vk](https://github.com/oqyh/cs2-Vote-GoldKingZ/assets/48490385/45e3352d-7b9d-4d56-810e-df7efba9ca3d)


## Todo List

- [x] Vote Kick 
- [ ] Vote Banned
- [ ] Vote Mute (Chat)
- [ ] Vote Gag (Voice)
- [ ] Vote Silent (Chat + Voice)
- [ ] Vote Send Play Spec
- [ ] Vote A Map

## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.
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

![colors](https://github.com/oqyh/cs2-vote-kick/assets/48490385/617503c9-fe77-480d-9ce2-fca5299cdcd5)



## .:[ Language ]:.
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
    //==========================
	
    "votekick.menu.name": "{purple}Vote Kick Menu",
    "votekick.player.vote.on.halfvotes.center.message": "{green}Gold KingZ {grey}| {darkred}Please Wait For Timer To End",
    "votekick.player.is.immunity": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Kick VIPs",//{0} Vip PlayerName 

    "votekick.player.vote.same.player": "{green}Gold KingZ {grey}| You've Already Voted To Kick {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]", //{0} PlayerName Vote On - {1} Votes - {2} Needed
    "votekick.player.vote.same.yes": "{green}Gold KingZ {grey}| You've Already Voted {lime}Yes {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]", //{0} PlayerName Vote On - {1} Votes - {2} Needed
    "votekick.player.vote.same.no": "{green}Gold KingZ {grey}| You've Already Voted {red}No {grey}To {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]", //{0} PlayerName Vote On - {1} Votes - {2} Needed
    
    "votekick.chat.message": " {green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted to kick {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]", //{0} PlayerName Rock The Vote - {1} PlayerName Vote On - {1} Votes - {2} Needed
    "votekick.announce.kick.successfully.message": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Kicked",{0} PlayerName Kicked

    "votekick.announce.halfvotes.chat.message": "{green}Gold KingZ {grey}| Votes Reached Half Type {yellow}!yes {grey}/ {yellow}!y {grey}Or {red}!no {grey}/ {red}!n {grey}To Vote Kick",
    "votekick.announce.halfvotes.center.message": "<font color='purple'>Vote Reach Half</font> <font color='darkred'>{0} Secs</font> <br> <font color='grey'>Kick player: </font> <font color='lightblue'>{1} ?</font> <br> <font class='fontSize-l' color='green'> [ {2} / {3} ] </font> <br> <font color='grey'>To Kick Say</font> <font color='yellow'>!yes</font><font color='grey'>/</font><font color='yellow'>!y</font> <br> <font color='grey'>To Remove Kick Say</font> <font color='yellow'>!no</font><font color='grey'>/</font><font color='yellow'>!n</font>"// {0} Timer - {1} PlayerName Vote On - {2} Votes - {3} Needed 
}
```

## .:[ Change Log ]:.
```
(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
