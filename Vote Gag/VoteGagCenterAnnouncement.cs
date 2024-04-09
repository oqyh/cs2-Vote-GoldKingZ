using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteGagCenterAnnouncement
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
            if(Configs.GetConfigData().VoteGag_TeamOnly)
            {
                if (Globals_VoteGag.VoteGag_ShowMenuCT.ContainsKey(playerid) && Globals_VoteGag.VoteGag_ShowMenuCT[playerid] && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (Globals_VoteGag.VoteGag_timerCT < 1 || Globals_VoteGag.VoteGag_countingCT >= Globals_VoteGag.VoteGag_requiredct)
                    {
                        Globals_VoteGag.VoteGag_timerCT = Configs.GetConfigData().VoteGag_CenterMessageAnnouncementTimer;
                        Globals_VoteGag.VoteGag_stopwatchCT.Stop();
                        Globals_VoteGag.VoteGag_ShowMenuCT.Clear();
                    }
                    
                    if (Globals_VoteGag.VoteGag_timerCT > 0)
                    {
                        if (Globals_VoteGag.VoteGag_stopwatchCT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteGag.VoteGag_timerCT--;
                            Globals_VoteGag.VoteGag_stopwatchCT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votegag.announce.halfvotes.center.message", Globals_VoteGag.VoteGag_timerCT,  Globals_VoteGag.VoteGag_targetPlayerNameCT, Globals_VoteGag.VoteGag_countingCT, Globals_VoteGag.VoteGag_requiredct]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
                if (Globals_VoteGag.VoteGag_ShowMenuT.ContainsKey(playerid) && Globals_VoteGag.VoteGag_ShowMenuT[playerid] && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (Globals_VoteGag.VoteGag_timerT < 1 || Globals_VoteGag.VoteGag_countingT >= Globals_VoteGag.VoteGag_requiredt)
                    {
                        Globals_VoteGag.VoteGag_timerT = Configs.GetConfigData().VoteGag_CenterMessageAnnouncementTimer;
                        Globals_VoteGag.VoteGag_stopwatchT.Stop();
                        Globals_VoteGag.VoteGag_ShowMenuT.Clear();
                    }
                    
                    if (Globals_VoteGag.VoteGag_timerT > 0)
                    {
                        if (Globals_VoteGag.VoteGag_stopwatchT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteGag.VoteGag_timerT--;
                            Globals_VoteGag.VoteGag_stopwatchT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votegag.announce.halfvotes.center.message", Globals_VoteGag.VoteGag_timerT,  Globals_VoteGag.VoteGag_targetPlayerNameT, Globals_VoteGag.VoteGag_countingT, Globals_VoteGag.VoteGag_requiredt]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }else
            {
                if (Globals_VoteGag.VoteGag_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteGag.VoteGag_ShowMenuBOTH[playerid])
                {
                    if (Globals_VoteGag.VoteGag_timerBOTH < 1 || Globals_VoteGag.VoteGag_countingBoth >= Globals_VoteGag.VoteGag_requiredboth)
                    {
                        Globals_VoteGag.VoteGag_timerBOTH = Configs.GetConfigData().VoteGag_CenterMessageAnnouncementTimer;
                        Globals_VoteGag.VoteGag_stopwatchBOTH.Stop();
                        Globals_VoteGag.VoteGag_ShowMenuBOTH.Clear();
                    }
                    
                    if (Globals_VoteGag.VoteGag_timerBOTH > 0)
                    {
                        if (Globals_VoteGag.VoteGag_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteGag.VoteGag_timerBOTH--;
                            Globals_VoteGag.VoteGag_stopwatchBOTH.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votegag.announce.halfvotes.center.message", Globals_VoteGag.VoteGag_timerBOTH,  Globals_VoteGag.VoteGag_targetPlayerNameBOTH, Globals_VoteGag.VoteGag_countingBoth, Globals_VoteGag.VoteGag_requiredboth]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }
            
        }
    }
}