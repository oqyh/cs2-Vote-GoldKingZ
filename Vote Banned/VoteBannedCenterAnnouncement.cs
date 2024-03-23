using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteBannedCenterAnnouncement
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
            if(Configs.GetConfigData().VoteBanned_TeamOnly)
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuCT.ContainsKey(playerid) && Globals_VoteBanned.VoteBanned_ShowMenuCT[playerid] && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (Globals_VoteKick.VoteKick_ShowMenuCT.ContainsKey(playerid) && Globals_VoteKick.VoteKick_ShowMenuCT[playerid])continue;
                    if (Globals_VoteBanned.VoteBanned_timerCT < 1 || Globals_VoteBanned.VoteBanned_countingCT >= Globals_VoteBanned.VoteBanned_requiredct)
                    {
                        Globals_VoteBanned.VoteBanned_timerCT = Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementTimer;
                        Globals_VoteBanned.VoteBanned_stopwatchCT.Stop();
                        Globals_VoteBanned.VoteBanned_ShowMenuCT.Clear();
                    }
                    
                    if (Globals_VoteBanned.VoteBanned_timerCT > 0)
                    {
                        if (Globals_VoteBanned.VoteBanned_stopwatchCT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteBanned.VoteBanned_timerCT--;
                            Globals_VoteBanned.VoteBanned_stopwatchCT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals_VoteBanned.VoteBanned_timerCT,  Globals_VoteBanned.VoteBanned_targetPlayerNameCT, Globals_VoteBanned.VoteBanned_countingCT, Globals_VoteBanned.VoteBanned_requiredct]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
                if (Globals_VoteBanned.VoteBanned_ShowMenuT.ContainsKey(playerid) && Globals_VoteBanned.VoteBanned_ShowMenuT[playerid] && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (Globals_VoteKick.VoteKick_ShowMenuT.ContainsKey(playerid) && Globals_VoteKick.VoteKick_ShowMenuT[playerid])continue;
                    if (Globals_VoteBanned.VoteBanned_timerT < 1 || Globals_VoteBanned.VoteBanned_countingT >= Globals_VoteBanned.VoteBanned_requiredt)
                    {
                        Globals_VoteBanned.VoteBanned_timerT = Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementTimer;
                        Globals_VoteBanned.VoteBanned_stopwatchT.Stop();
                        Globals_VoteBanned.VoteBanned_ShowMenuT.Clear();
                    }
                    
                    if (Globals_VoteBanned.VoteBanned_timerT > 0)
                    {
                        if (Globals_VoteBanned.VoteBanned_stopwatchT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteBanned.VoteBanned_timerT--;
                            Globals_VoteBanned.VoteBanned_stopwatchT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals_VoteBanned.VoteBanned_timerT,  Globals_VoteBanned.VoteBanned_targetPlayerNameT, Globals_VoteBanned.VoteBanned_countingT, Globals_VoteBanned.VoteBanned_requiredt]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }else
            {
                if (Globals_VoteBanned.VoteBanned_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteBanned.VoteBanned_ShowMenuBOTH[playerid])
                {
                    if (Globals_VoteKick.VoteKick_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteKick.VoteKick_ShowMenuBOTH[playerid])continue;
                    if (Globals_VoteBanned.VoteBanned_timerBOTH < 1 || Globals_VoteBanned.VoteBanned_countingBoth >= Globals_VoteBanned.VoteBanned_requiredboth)
                    {
                        Globals_VoteBanned.VoteBanned_timerBOTH = Configs.GetConfigData().VoteBanned_CenterMessageAnnouncementTimer;
                        Globals_VoteBanned.VoteBanned_stopwatchBOTH.Stop();
                        Globals_VoteBanned.VoteBanned_ShowMenuBOTH.Clear();
                    }
                    
                    if (Globals_VoteBanned.VoteBanned_timerBOTH > 0)
                    {
                        if (Globals_VoteBanned.VoteBanned_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteBanned.VoteBanned_timerBOTH--;
                            Globals_VoteBanned.VoteBanned_stopwatchBOTH.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals_VoteBanned.VoteBanned_timerBOTH,  Globals_VoteBanned.VoteBanned_targetPlayerNameBOTH, Globals_VoteBanned.VoteBanned_countingBoth, Globals_VoteBanned.VoteBanned_requiredboth]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }
            
        }
    }
}