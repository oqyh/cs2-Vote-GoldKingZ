using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteGag
{
    public static float VoteGag_timerCT;
    public static float VoteGag_timerT;
    public static float VoteGag_timerBOTH;

    public static Stopwatch VoteGag_stopwatchCT = new Stopwatch();
    public static Stopwatch VoteGag_stopwatchT = new Stopwatch();
    public static Stopwatch VoteGag_stopwatchBOTH = new Stopwatch();
    public static string VoteGag_targetPlayerNameCT = "";
    public static string VoteGag_targetPlayerNameT = "";
    public static string VoteGag_targetPlayerNameBOTH = "";
    public static string VoteGag_targetPlayerIPCT = "";
    public static string VoteGag_targetPlayerIPT = "";
    public static string VoteGag_targetPlayerIPBOTH = "";
    public static ulong VoteGag_targetPlayerSTEAMCT;
    public static ulong VoteGag_targetPlayerSTEAMT;
    public static ulong VoteGag_targetPlayerSTEAMBOTH;
    public static bool VoteGag_Disabled = false;
    public static bool VoteGag_ReachHalfVoteCT = false;
    public static bool VoteGag_ReachHalfVoteT = false;
    public static bool VoteGag_ReachHalfVoteBoth = false;
    public static int VoteGag_countingCT;
    public static int VoteGag_countingT;
    public static int VoteGag_countingBoth;
    public static int VoteGag_requiredct;
    public static int VoteGag_requiredt;
    public static int VoteGag_requiredboth;
    public static Dictionary<ulong, bool> VoteGag_ShowMenuCT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteGag_ShowMenuT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteGag_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteGag_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteGag_Disable = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteGag_GetVoted = new Dictionary<string, int>();
    public static Dictionary<ulong, bool> VoteGag_PlayerGaged = new Dictionary<ulong, bool>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteGag_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}