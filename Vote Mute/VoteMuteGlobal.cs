using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteMute
{
    public static float VoteMute_timerCT;
    public static float VoteMute_timerT;
    public static float VoteMute_timerBOTH;

    public static Stopwatch VoteMute_stopwatchCT = new Stopwatch();
    public static Stopwatch VoteMute_stopwatchT = new Stopwatch();
    public static Stopwatch VoteMute_stopwatchBOTH = new Stopwatch();
    public static string VoteMute_targetPlayerNameCT = "";
    public static string VoteMute_targetPlayerNameT = "";
    public static string VoteMute_targetPlayerNameBOTH = "";
    public static string VoteMute_targetPlayerIPCT = "";
    public static string VoteMute_targetPlayerIPT = "";
    public static string VoteMute_targetPlayerIPBOTH = "";
    public static ulong VoteMute_targetPlayerSTEAMCT;
    public static ulong VoteMute_targetPlayerSTEAMT;
    public static ulong VoteMute_targetPlayerSTEAMBOTH;
    public static bool VoteMute_ReachHalfVoteCT = false;
    public static bool VoteMute_ReachHalfVoteT = false;
    public static bool VoteMute_ReachHalfVoteBoth = false;
    public static int VoteMute_countingCT;
    public static int VoteMute_countingT;
    public static int VoteMute_countingBoth;
    public static int VoteMute_requiredct;
    public static int VoteMute_requiredt;
    public static int VoteMute_requiredboth;
    public static Dictionary<ulong, bool> VoteMute_ShowMenuCT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteMute_ShowMenuT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteMute_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteMute_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteMute_GetVoted = new Dictionary<string, int>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteMute_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}