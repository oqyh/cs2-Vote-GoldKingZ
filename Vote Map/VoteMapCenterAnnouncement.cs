using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteMapCenterAnnouncement
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
            
            if (Globals_VoteMap.VoteMap_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteMap.VoteMap_ShowMenuBOTH[playerid])
            {
                if (Globals_VoteMap.VoteMap_timerBOTH < 1 || Globals_VoteMap.VoteMap_countingBoth >= Globals_VoteMap.VoteMap_requiredboth)
                {
                    Globals_VoteMap.VoteMap_timerBOTH = Configs.GetConfigData().VoteMap_CenterMessageAnnouncementTimer;
                    Globals_VoteMap.VoteMap_stopwatchBOTH.Stop();
                    Globals_VoteMap.VoteMap_ShowMenuBOTH.Clear();
                }
                
                if (Globals_VoteMap.VoteMap_timerBOTH > 0)
                {
                    if (Globals_VoteMap.VoteMap_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                    {
                        Globals_VoteMap.VoteMap_timerBOTH--;
                        Globals_VoteMap.VoteMap_stopwatchBOTH.Restart();
                    }
                }
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(Localizer!["votemap.announce.halfvotes.center.message", Globals_VoteMap.VoteMap_timerBOTH,  Globals_VoteMap.VoteMap_targetPlayerNameBOTH, Globals_VoteMap.VoteMap_countingBoth, Globals_VoteMap.VoteMap_requiredboth]);
                var centerhtml = builder.ToString();
                player.PrintToCenterHtml(centerhtml);
                
            }
            
            
        }
    }
}