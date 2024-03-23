using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteKickCenterAnnouncement
{
    private IStringLocalizer? Localizer;
    public void SetStringLocalizer(IStringLocalizer stringLocalizer)
    {
        Localizer = stringLocalizer;
    }

    public void OnTick()
    {

        var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
        foreach (var player in playerEntities)
        {
            if (player == null || !player.IsValid || !player.PawnIsAlive || player.IsBot || player.IsHLTV) continue;
            var playerid = player.SteamID;
            if(Configs.GetConfigData().VoteKick_TeamOnly)
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(playerid) && Globals_VoteBanned.VoteBanned_ShowMenuCT[playerid])continue;
                if (Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(playerid) && Globals_VoteKick.VoteKick_ShowMenuCT[playerid] && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (Globals_VoteKick.VoteKick_timerCT < 1 || Globals_VoteKick.VoteKick_countingCT >= Globals_VoteKick.VoteKick_requiredct)
                    {
                        Globals_VoteKick.VoteKick_timerCT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
                        Globals_VoteKick.VoteKick_stopwatchCT.Stop();
                        Globals_VoteKick.VoteKick_ShowMenuCT.Clear();
                    }
                    
                    if (Globals_VoteKick.VoteKick_timerCT > 0)
                    {
                        if (Globals_VoteKick.VoteKick_stopwatchCT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteKick.VoteKick_timerCT--;
                            Globals_VoteKick.VoteKick_stopwatchCT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals_VoteKick.VoteKick_timerCT,  Globals_VoteKick.VoteKick_targetPlayerNameCT, Globals_VoteKick.VoteKick_countingCT, Globals_VoteKick.VoteKick_requiredct]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
                if (Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(playerid) && Globals_VoteKick.VoteKick_ShowMenuT[playerid] && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(playerid) && Globals_VoteBanned.VoteBanned_ShowMenuT[playerid])continue;
                    if (Globals_VoteKick.VoteKick_timerT < 1 || Globals_VoteKick.VoteKick_countingT >= Globals_VoteKick.VoteKick_requiredt)
                    {
                        Globals_VoteKick.VoteKick_timerT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
                        Globals_VoteKick.VoteKick_stopwatchT.Stop();
                        Globals_VoteKick.VoteKick_ShowMenuT.Clear();
                    }
                    
                    if (Globals_VoteKick.VoteKick_timerT > 0)
                    {
                        if (Globals_VoteKick.VoteKick_stopwatchT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteKick.VoteKick_timerT--;
                            Globals_VoteKick.VoteKick_stopwatchT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals_VoteKick.VoteKick_timerT,  Globals_VoteKick.VoteKick_targetPlayerNameT, Globals_VoteKick.VoteKick_countingT, Globals_VoteKick.VoteKick_requiredt]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }else
            {
                if (Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteKick.VoteKick_ShowMenuBOTH[playerid])
                {
                    if (Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteBanned.VoteBanned_ShowMenuBOTH[playerid])continue;
                    if (Globals_VoteKick.VoteKick_timerBOTH < 1 || Globals_VoteKick.VoteKick_countingBoth >= Globals_VoteKick.VoteKick_requiredboth)
                    {
                        Globals_VoteKick.VoteKick_timerBOTH = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
                        Globals_VoteKick.VoteKick_stopwatchBOTH.Stop();
                        Globals_VoteKick.VoteKick_ShowMenuBOTH.Clear();
                    }
                    
                    if (Globals_VoteKick.VoteKick_timerBOTH > 0)
                    {
                        if (Globals_VoteKick.VoteKick_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteKick.VoteKick_timerBOTH--;
                            Globals_VoteKick.VoteKick_stopwatchBOTH.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals_VoteKick.VoteKick_timerBOTH,  Globals_VoteKick.VoteKick_targetPlayerNameBOTH, Globals_VoteKick.VoteKick_countingBoth, Globals_VoteKick.VoteKick_requiredboth]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }
            
        }
    }
}