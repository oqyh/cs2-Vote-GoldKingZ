using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteMap
{
    public static float VoteMap_timerBOTH;
    public static Stopwatch VoteMap_stopwatchBOTH = new Stopwatch();
    public static string VoteMap_targetPlayerNameBOTH = "";
    public static string VoteMap_targetPlayerIPBOTH = "";
    public static ulong VoteMap_targetPlayerSTEAMBOTH;
    public static bool VoteMap_Disabled = false;
    public static bool VoteMap_Timer = false;
    public static bool VoteMap_ReachHalfVoteBoth = false;
    public static int VoteMap_countingBoth;
    public static int VoteMap_requiredboth;
    public static Dictionary<ulong, bool> VoteMap_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteMap_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteMap_Disable = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteMap_GetVoted = new Dictionary<string, int>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteMap_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}