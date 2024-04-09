using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Vote_GoldKingZ;

public class Globals_VoteGameMode
{
    public static float VoteGameMode_timerBOTH;
    public static Stopwatch VoteGameMode_stopwatchBOTH = new Stopwatch();
    public static string VoteGameMode_targetPlayerNameBOTH = "";
    public static string VoteGameMode_targetPlayerIPBOTH = "";
    public static ulong VoteGameMode_targetPlayerSTEAMBOTH;
    public static bool VoteGameMode_Disabled = false;
    public static bool VoteGameMode_ReachHalfVoteBoth = false;
    public static int VoteGameMode_countingBoth;
    public static int VoteGameMode_requiredboth;
    public static Dictionary<ulong, bool> VoteGameMode_ShowMenuBOTH = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteGameMode_Immunity = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VoteGameMode_Disable = new Dictionary<ulong, bool>();
    public static Dictionary<string, int> VoteGameMode_GetVoted = new Dictionary<string, int>();
    public static Dictionary<CCSPlayerController, HashSet<string>> VoteGameMode_CallerVotedTo = new Dictionary<CCSPlayerController, HashSet<string>>(); 
}