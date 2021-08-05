using System.Linq;
using rewind.Player;
using rewind.Rewindable;
using rewind.Rewindable.Npc;
using Sandbox;

namespace rewind
{
	public partial class RewindGame
	{
		[ServerCmd("rewind_debug_npcs")]
		public static void SpawnDebugNpcs(int cnt)
		{
			RewindableNpc.SpawnNpcs( cnt, All.OfType<Sandbox.Player>().First().Position );
		}
	}
}
