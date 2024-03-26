using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteMuteCenterAnnouncement
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
            if(Configs.GetConfigData().VoteMute_TeamOnly)
            {
                if (Globals_VoteMute.VoteMute_ShowMenuCT.ContainsKey(playerid) && Globals_VoteMute.VoteMute_ShowMenuCT[playerid] && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (Globals_VoteMute.VoteMute_timerCT < 1 || Globals_VoteMute.VoteMute_countingCT >= Globals_VoteMute.VoteMute_requiredct)
                    {
                        Globals_VoteMute.VoteMute_timerCT = Configs.GetConfigData().VoteMute_CenterMessageAnnouncementTimer;
                        Globals_VoteMute.VoteMute_stopwatchCT.Stop();
                        Globals_VoteMute.VoteMute_ShowMenuCT.Clear();
                    }
                    
                    if (Globals_VoteMute.VoteMute_timerCT > 0)
                    {
                        if (Globals_VoteMute.VoteMute_stopwatchCT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteMute.VoteMute_timerCT--;
                            Globals_VoteMute.VoteMute_stopwatchCT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votemute.announce.halfvotes.center.message", Globals_VoteMute.VoteMute_timerCT,  Globals_VoteMute.VoteMute_targetPlayerNameCT, Globals_VoteMute.VoteMute_countingCT, Globals_VoteMute.VoteMute_requiredct]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
                if (Globals_VoteMute.VoteMute_ShowMenuT.ContainsKey(playerid) && Globals_VoteMute.VoteMute_ShowMenuT[playerid] && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (Globals_VoteMute.VoteMute_timerT < 1 || Globals_VoteMute.VoteMute_countingT >= Globals_VoteMute.VoteMute_requiredt)
                    {
                        Globals_VoteMute.VoteMute_timerT = Configs.GetConfigData().VoteMute_CenterMessageAnnouncementTimer;
                        Globals_VoteMute.VoteMute_stopwatchT.Stop();
                        Globals_VoteMute.VoteMute_ShowMenuT.Clear();
                    }
                    
                    if (Globals_VoteMute.VoteMute_timerT > 0)
                    {
                        if (Globals_VoteMute.VoteMute_stopwatchT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteMute.VoteMute_timerT--;
                            Globals_VoteMute.VoteMute_stopwatchT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votemute.announce.halfvotes.center.message", Globals_VoteMute.VoteMute_timerT,  Globals_VoteMute.VoteMute_targetPlayerNameT, Globals_VoteMute.VoteMute_countingT, Globals_VoteMute.VoteMute_requiredt]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }else
            {
                if (Globals_VoteMute.VoteMute_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteMute.VoteMute_ShowMenuBOTH[playerid])
                {
                    if (Globals_VoteMute.VoteMute_timerBOTH < 1 || Globals_VoteMute.VoteMute_countingBoth >= Globals_VoteMute.VoteMute_requiredboth)
                    {
                        Globals_VoteMute.VoteMute_timerBOTH = Configs.GetConfigData().VoteMute_CenterMessageAnnouncementTimer;
                        Globals_VoteMute.VoteMute_stopwatchBOTH.Stop();
                        Globals_VoteMute.VoteMute_ShowMenuBOTH.Clear();
                    }
                    
                    if (Globals_VoteMute.VoteMute_timerBOTH > 0)
                    {
                        if (Globals_VoteMute.VoteMute_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteMute.VoteMute_timerBOTH--;
                            Globals_VoteMute.VoteMute_stopwatchBOTH.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votemute.announce.halfvotes.center.message", Globals_VoteMute.VoteMute_timerBOTH,  Globals_VoteMute.VoteMute_targetPlayerNameBOTH, Globals_VoteMute.VoteMute_countingBoth, Globals_VoteMute.VoteMute_requiredboth]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }
            
        }
    }
}