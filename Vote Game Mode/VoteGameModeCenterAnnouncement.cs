using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using Vote_GoldKingZ.Config;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;

namespace Vote_GoldKingZ;

public class VoteGameModeCenterAnnouncement
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
            
            if (Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.ContainsKey(playerid) && Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH[playerid])
            {
                if (Globals_VoteGameMode.VoteGameMode_timerBOTH < 1 || Globals_VoteGameMode.VoteGameMode_countingBoth >= Globals_VoteGameMode.VoteGameMode_requiredboth)
                {
                    Globals_VoteGameMode.VoteGameMode_timerBOTH = Configs.GetConfigData().VoteGameMode_CenterMessageAnnouncementTimer;
                    Globals_VoteGameMode.VoteGameMode_stopwatchBOTH.Stop();
                    Globals_VoteGameMode.VoteGameMode_ShowMenuBOTH.Clear();
                }
                
                if (Globals_VoteGameMode.VoteGameMode_timerBOTH > 0)
                {
                    if (Globals_VoteGameMode.VoteGameMode_stopwatchBOTH.ElapsedMilliseconds >= 1000)
                    {
                        Globals_VoteGameMode.VoteGameMode_timerBOTH--;
                        Globals_VoteGameMode.VoteGameMode_stopwatchBOTH.Restart();
                    }
                }
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(Localizer!["votegamemode.announce.halfvotes.center.message", Globals_VoteGameMode.VoteGameMode_timerBOTH,  Globals_VoteGameMode.VoteGameMode_targetPlayerNameBOTH, Globals_VoteGameMode.VoteGameMode_countingBoth, Globals_VoteGameMode.VoteGameMode_requiredboth]);
                var centerhtml = builder.ToString();
                player.PrintToCenterHtml(centerhtml);
                
            }
            
            
        }
    }
}