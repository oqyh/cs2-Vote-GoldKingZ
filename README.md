# [CS2] Vote-Kick (1.0.0)

### Vote Kick Players With SteamID/IpAddress Restrict From Joining


![hud](https://github.com/oqyh/cs2-vote-kick/assets/48490385/8ae6ab2b-6772-48fb-8737-4a6aa479c3f1)

![chat](https://github.com/oqyh/cs2-vote-kick/assets/48490385/b6484550-9d44-46d8-aee0-ee75f4e8ef4a)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)


## .:[ Configuration ]:.
```json
{
  //Immunity From Getting Vote To Kick
  "ImmunityGroupsFromGettingVoted": "#css/vip1,#css/vip2,#css/vip3",
  
  //Make Vote Kick TeamSide Only
  "VoteTeamMateOnly": false,
  
  //Vote Percentage To Pass The Vote 6 means 0.6 || 5 means 0.5 Half
  "VotePercentage": 6,
  
  //If Vote Reach Half Do You Want Annoce Player To Vote shortcut !yes/!y or !no/n To Kick Player Annoce
  "VoteMessageAnnounceOnHalfVotes": false,
  
  //if VoteMessageAnnounceOnHalfVotes Enabled How Many In Secs To Show Message
  "VoteTimer": 25,
  
//-----------------------------------------------------------------------------------------

  //After Kicking Player Which Method Do You Like
  //RestrictPlayersMethod (0) = Kick Only
  //RestrictPlayersMethod (1) = Kick And Restrict SteamID From Joining
  //RestrictPlayersMethod (2) = Kick And Restrict IpAddress From Joining
  //RestrictPlayersMethod (3) = Kick And Restrict SteamID And IpAddress From Joining
  "RestrictPlayersMethod": 1,
  
  //If Vote Pass How Many In Mins Should Kicked Player Wait To Join Back
  "AfterKickGivePlayerXMinsFromJoining": 5,
  
  //Rest And Cancel AfterKickGivePlayerXMinsFromJoining On Map Change
  "ResetKickedPlayersOnMapChange": false,
  
//-----------------------------------------------------------------------------------------
  "ConfigVersion": 1
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
	"Vote_YourSelf": "{green}Gold KingZ {grey}| You Cant Vote Kick Your Self",
    "This_User_IS_VIP": "{green}Gold KingZ {grey}| {darkred}Vote Failed On {Purple}{0} {darkred}You Cant Vote Kick VIPs", //========={0} = Vip Player Name
    "Vote_TeamMateOnly": "{green}Gold KingZ {grey}| You Cant Vote Kick Opposite Team",
    "Vote_KickMessage": "{green}Gold KingZ {grey}| {Purple}{0} {grey}Wanted to kick {Magenta}{1} {grey}[ {Olive}{2} {grey}/ {Olive}{3} {grey}]", //========={0} = Name Who Called The Vote || {1} = Name Who Got Voted On || {2} = How Many Total Votes On Voted || {3} = How Many Needed 
    "Vote_KickMessageDuplicate": "{green}Gold KingZ {grey}| You've Already Voted To Kick {Purple}{0} {grey}[ {Olive}{1} {grey}/ {Olive}{2} {grey}]", //========={0} = Name Who Got Voted On || {1} = How Many Total Votes On Voted || {2} = How Many Needed 
    "Vote_KickMessagePassed": "{green}Gold KingZ {grey}| Vote Successfully, {Purple}{0} {grey}Has Been Kicked" //========={0} = Name Who Got Successfully Kicked
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
