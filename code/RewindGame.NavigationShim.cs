using System.Linq;
using rewind.Player;
using rewind.Rewindable;
using rewind.Rewindable.Npc;
using rewind.Rewindable.Npc.Tasks;
using Sandbox;

namespace rewind
{
	// Methods used to relay navigation data to the client
	public partial class RewindGame
	{
		[ServerCmd]
		public static void RequestPath( int entId, Vector3 startPos, Vector3 endPos )
		{
			Host.AssertServer();
			
			//Log.Info( $"RequestPath for {entId}" );
			ReportPathToNpc( entId, NavMesh.BuildPath( startPos, endPos ) );
		}
		
		[ClientRpc]
		public static void ReportPathToNpc( int entId, Vector3[] path )
		{
			Host.AssertClientOrMenu();
			
			var ent = Entity.All.FirstOrDefault( x => x.NetworkIdent == entId ) as RewindableNpc;
			
			if ( ent is {CurrentTask: WalkTask t} && path != null )
			{
				//Log.Info( $"ReportPathToNpc for {entId}: Set {path.Length} points" );
				
				t.Points = path.ToList();
			}
		}
	}
}
