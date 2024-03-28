using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteBanned
{
    public static float VoteBanned_timerCT;
    public static float VoteBanned_timerT;
    public static float VoteBanned_timerBOTH;

    public static Stopwatch VoteBanned_stopwatchCT = new Stopwatch();
    public static Stopwatch VoteBanned_stopwatchT = new Stopwatch();
    public static Stopwatch VoteBanned_stopwatchBOTH = new Stopwatch();
    public static string VoteBanned_targetPlayerNameCT = "";
    public static string VoteBanned_targetPlayerNameT = "";
    public static string VoteBanned_targetPlayerNameBOTH = "";
    public static string VoteBanned_targetPlayerIPCT = "";
    public static string VoteBanned_targetPlayerIPT = "";
    public static string VoteBanned_targetPlayerIPBOTH = "";
    public static ulong VoteBanned_targetPlayerSTEAMCT;
    public static ulong VoteBanned_targetPlayerSTEAMT;
    public static ulong VoteBanned_targetPlayerSTEAMBOTH;
    public static bool VoteBanned_Disabled = false;
    public static bool VoteBanned_ReachHalfVoteCT = false;
    public static bool VoteBanned_ReachHalfVoteT = false;
    public static bool VoteBanned_ReachHalfVoteBoth = false;
    public static int VoteBanned_countingCT;
    public static int VoteBanned_countingT;
    public static int VoteBanned_countingBoth;
    public static int VoteBanned_requiredct;
    public static int VoteBanned_requiredt;
    public static int VoteBanned_requiredboth;
    public static Dictionary<ulong, bool> VoteBanned_ShowMenuCT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteBanned_ShowMenuT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteBanned_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteBanned_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteBanned_Disable = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteBanned_GetVoted = new Dictionary<string, int>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteBanned_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}