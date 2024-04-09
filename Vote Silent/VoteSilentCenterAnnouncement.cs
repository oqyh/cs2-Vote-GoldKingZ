using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteSilentCenterAnnouncement
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
            if(Configs.GetConfigData().VoteSilent_TeamOnly)
            {
                if (Globals_VoteSilent.VoteSilent_ShowMenuCT.ContainsKey(playerid) && Globals_VoteSilent.VoteSilent_ShowMenuCT[playerid] && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    if (Globals_VoteSilent.VoteSilent_timerCT < 1 || Globals_VoteSilent.VoteSilent_countingCT >= Globals_VoteSilent.VoteSilent_requiredct)
                    {
                        Globals_VoteSilent.VoteSilent_timerCT = Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementTimer;
                        Globals_VoteSilent.VoteSilent_stopwatchCT.Stop();
                        Globals_VoteSilent.VoteSilent_ShowMenuCT.Clear();
                    }
                    
                    if (Globals_VoteSilent.VoteSilent_timerCT > 0)
                    {
                        if (Globals_VoteSilent.VoteSilent_stopwatchCT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteSilent.VoteSilent_timerCT--;
                            Globals_VoteSilent.VoteSilent_stopwatchCT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votesilent.announce.halfvotes.center.message", Globals_VoteSilent.VoteSilent_timerCT,  Globals_VoteSilent.VoteSilent_targetPlayerNameCT, Globals_VoteSilent.VoteSilent_countingCT, Globals_VoteSilent.VoteSilent_requiredct]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
                if (Globals_VoteSilent.VoteSilent_ShowMenuT.ContainsKey(playerid) && Globals_VoteSilent.VoteSilent_ShowMenuT[playerid] && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    if (Globals_VoteSilent.VoteSilent_timerT < 1 || Globals_VoteSilent.VoteSilent_countingT >= Globals_VoteSilent.VoteSilent_requiredt)
                    {
                        Globals_VoteSilent.VoteSilent_timerT = Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementTimer;
                        Globals_VoteSilent.VoteSilent_stopwatchT.Stop();
                        Globals_VoteSilent.VoteSilent_ShowMenuT.Clear();
                    }
                    
                    if (Globals_VoteSilent.VoteSilent_timerT > 0)
                    {
                        if (Globals_VoteSilent.VoteSilent_stopwatchT.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteSilent.VoteSilent_timerT--;
                            Globals_VoteSilent.VoteSilent_stopwatchT.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votesilent.announce.halfvotes.center.message", Globals_VoteSilent.VoteSilent_timerT,  Globals_VoteSilent.VoteSilent_targetPlayerNameT, Globals_VoteSilent.VoteSilent_countingT, Globals_VoteSilent.VoteSilent_requiredt]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }else
            {
                if (Globals_VoteSilent.VoteSilent_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteSilent.VoteSilent_ShowMenuBOTH[playerid])
                {
                    if (Globals_VoteSilent.VoteSilent_timerBOTH < 1 || Globals_VoteSilent.VoteSilent_countingBoth >= Globals_VoteSilent.VoteSilent_requiredboth)
                    {
                        Globals_VoteSilent.VoteSilent_timerBOTH = Configs.GetConfigData().VoteSilent_CenterMessageAnnouncementTimer;
                        Globals_VoteSilent.VoteSilent_stopwatchBOTH.Stop();
                        Globals_VoteSilent.VoteSilent_ShowMenuBOTH.Clear();
                    }
                    
                    if (Globals_VoteSilent.VoteSilent_timerBOTH > 0)
                    {
                        if (Globals_VoteSilent.VoteSilent_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                        {
                            Globals_VoteSilent.VoteSilent_timerBOTH--;
                            Globals_VoteSilent.VoteSilent_stopwatchBOTH.Restart();
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer!["votesilent.announce.halfvotes.center.message", Globals_VoteSilent.VoteSilent_timerBOTH,  Globals_VoteSilent.VoteSilent_targetPlayerNameBOTH, Globals_VoteSilent.VoteSilent_countingBoth, Globals_VoteSilent.VoteSilent_requiredboth]);
                    var centerhtml = builder.ToString();
                    player.PrintToCenterHtml(centerhtml);
                    
                }
            }
            
        }
    }
}