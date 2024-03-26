using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteKick
{
    public static float VoteKick_timerCT;
    public static float VoteKick_timerT;
    public static float VoteKick_timerBOTH;

    public static Stopwatch VoteKick_stopwatchCT = new Stopwatch();
    public static Stopwatch VoteKick_stopwatchT = new Stopwatch();
    public static Stopwatch VoteKick_stopwatchBOTH = new Stopwatch();
    public static string VoteKick_targetPlayerNameCT = "";
    public static string VoteKick_targetPlayerNameT = "";
    public static string VoteKick_targetPlayerNameBOTH = "";
    public static string VoteKick_targetPlayerIPCT = "";
    public static string VoteKick_targetPlayerIPT = "";
    public static string VoteKick_targetPlayerIPBOTH = "";
    public static ulong VoteKick_targetPlayerSTEAMCT;
    public static ulong VoteKick_targetPlayerSTEAMT;
    public static ulong VoteKick_targetPlayerSTEAMBOTH;
    public static bool VoteKick_ReachHalfVoteCT = false;
    public static bool VoteKick_ReachHalfVoteT = false;
    public static bool VoteKick_ReachHalfVoteBoth = false;
    public static int VoteKick_countingCT;
    public static int VoteKick_countingT;
    public static int VoteKick_countingBoth;
    public static int VoteKick_requiredct;
    public static int VoteKick_requiredt;
    public static int VoteKick_requiredboth;
    public static Dictionary<ulong, bool> VoteKick_Delaykick = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteKick_ShowMenuCT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteKick_ShowMenuT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteKick_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteKick_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteKick_GetVoted = new Dictionary<string, int>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteKick_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}