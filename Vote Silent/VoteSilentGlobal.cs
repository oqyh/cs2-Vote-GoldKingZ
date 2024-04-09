using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteSilent
{
    public static float VoteSilent_timerCT;
    public static float VoteSilent_timerT;
    public static float VoteSilent_timerBOTH;

    public static Stopwatch VoteSilent_stopwatchCT = new Stopwatch();
    public static Stopwatch VoteSilent_stopwatchT = new Stopwatch();
    public static Stopwatch VoteSilent_stopwatchBOTH = new Stopwatch();
    public static string VoteSilent_targetPlayerNameCT = "";
    public static string VoteSilent_targetPlayerNameT = "";
    public static string VoteSilent_targetPlayerNameBOTH = "";
    public static string VoteSilent_targetPlayerIPCT = "";
    public static string VoteSilent_targetPlayerIPT = "";
    public static string VoteSilent_targetPlayerIPBOTH = "";
    public static ulong VoteSilent_targetPlayerSTEAMCT;
    public static ulong VoteSilent_targetPlayerSTEAMT;
    public static ulong VoteSilent_targetPlayerSTEAMBOTH;
    public static bool VoteSilent_Disabled = false;
    public static bool VoteSilent_ReachHalfVoteCT = false;
    public static bool VoteSilent_ReachHalfVoteT = false;
    public static bool VoteSilent_ReachHalfVoteBoth = false;
    public static int VoteSilent_countingCT;
    public static int VoteSilent_countingT;
    public static int VoteSilent_countingBoth;
    public static int VoteSilent_requiredct;
    public static int VoteSilent_requiredt;
    public static int VoteSilent_requiredboth;
    public static Dictionary<ulong, bool> VoteSilent_ShowMenuCT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteSilent_ShowMenuT = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteSilent_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteSilent_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteSilent_Disable = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteSilent_GetVoted = new Dictionary<string, int>();
	public static Dictionary<ulong, bool> VoteSilent_PlayerGaged = new Dictionary<ulong, bool>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteSilent_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}