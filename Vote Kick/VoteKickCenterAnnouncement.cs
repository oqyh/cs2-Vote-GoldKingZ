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
                if (Globals.VoteKick_ShowMenuCT.ContainsKey(playerid) && Globals.VoteKick_ShowMenuCT[playerid] && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (Globals.VoteKick_timerCT < 1 || Globals.VoteKick_countingCT >= Globals.VoteKick_requiredct)
                    {
                        Globals.VoteKick_timerCT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
                        Globals.VoteKick_stopwatchCT.Stop();
                        Globals.VoteKick_ShowMenuCT.Clear();
                    }
                    
                    if (Globals.VoteKick_timerCT > 0)
                    {
                        if (Globals.VoteKick_stopwatchCT.ElapsedMilliseconds >= 1000)
                        {
                            Globals.VoteKick_timerCT--;
                            Globals.VoteKick_stopwatchCT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals.VoteKick_timerCT,  Globals.VoteKick_targetPlayerNameCT, Globals.VoteKick_countingCT, Globals.VoteKick_requiredct]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
                if (Globals.VoteKick_ShowMenuT.ContainsKey(playerid) && Globals.VoteKick_ShowMenuT[playerid] && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (Globals.VoteKick_timerT < 1 || Globals.VoteKick_countingT >= Globals.VoteKick_requiredt)
                    {
                        Globals.VoteKick_timerT = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
                        Globals.VoteKick_stopwatchT.Stop();
                        Globals.VoteKick_ShowMenuT.Clear();
                    }
                    
                    if (Globals.VoteKick_timerT > 0)
                    {
                        if (Globals.VoteKick_stopwatchT.ElapsedMilliseconds >= 1000)
                        {
                            Globals.VoteKick_timerT--;
                            Globals.VoteKick_stopwatchT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals.VoteKick_timerT,  Globals.VoteKick_targetPlayerNameT, Globals.VoteKick_countingT, Globals.VoteKick_requiredt]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }else
            {
                if (Globals.VoteKick_ShowMenuBOTH.ContainsKey(playerid) && Globals.VoteKick_ShowMenuBOTH[playerid])
                {
                    if (Globals.VoteKick_timerBOTH < 1 || Globals.VoteKick_countingBoth >= Globals.VoteKick_requiredboth)
                    {
                        Globals.VoteKick_timerBOTH = Configs.GetConfigData().VoteKick_CenterMessageAnnouncementTimer;
                        Globals.VoteKick_stopwatchBOTH.Stop();
                        Globals.VoteKick_ShowMenuBOTH.Clear();
                    }
                    
                    if (Globals.VoteKick_timerBOTH > 0)
                    {
                        if (Globals.VoteKick_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                        {
                            Globals.VoteKick_timerBOTH--;
                            Globals.VoteKick_stopwatchBOTH.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votekick.announce.halfvotes.center.message", Globals.VoteKick_timerBOTH,  Globals.VoteKick_targetPlayerNameBOTH, Globals.VoteKick_countingBoth, Globals.VoteKick_requiredboth]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }
            
        }
    }
}