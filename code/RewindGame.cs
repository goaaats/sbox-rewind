
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using rewind;
using rewind.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace rewind
{
	[Library]
	public partial class RewindGame : Sandbox.Game
	{
		public const int MAX_TRACKED_FRAGMENTS = 5000;

		private static RewindMode modeInternal = RewindMode.Gameplay;

		public static RewindMode Mode
		{
			get => modeInternal;
			set
			{
				modeInternal = value;
				NotifyStateChange( value );
			}
		}

		private static void NotifyStateChange( RewindMode mode )
		{
			foreach (var entity in RewindableProp.All.Where( x => x is IRewindable ).Cast<IRewindable>())
			{
				entity.UpdateRewindState(mode);
			}
		}

		private const int TrackedDeltaTimes = 100;
		private static float[] prevDeltaTimes = new float[TrackedDeltaTimes];
		private static  int curTrackedIndex = 0;

		public static float SmoothDeltaTime => prevDeltaTimes.Aggregate( ( x, y ) => x + y ) / TrackedDeltaTimes;

		public RewindGame()
		{
			if ( IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );
				
				new RewindHudEntity();
			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
				
			}
		}

		public override void FrameSimulate( Client cl )
		{
			foreach (var entity in RewindableProp.All.Where( x => x is IRewindable ).Cast<IRewindable>())
			{
				entity.RewindSimulate();
			}

			prevDeltaTimes[curTrackedIndex] = Time.Delta;
			curTrackedIndex++;
			if ( curTrackedIndex > TrackedDeltaTimes - 1 )
				curTrackedIndex = 0;
			
			DebugOverlay.ScreenText( 6, "MinCnt: " + GetMinRewindableSimulates() );
			DebugOverlay.ScreenText( 7, $"MinSec: {GetMinRewindableSeconds():00.0} / {GetMaxRewindableSeconds():00.0} " );
			DebugOverlay.ScreenText( 8, $"SmoothDeltaTime: {SmoothDeltaTime}" );

			base.FrameSimulate( cl );
		}

		public static int GetMinRewindableSimulates()
		{
			var highest = 0;
			
			foreach (var entity in All.Where( x => x is IRewindable ).Cast<IRewindable>())
			{
				if ( entity.Fragments.Count > highest )
					highest = entity.Fragments.Count;
			}

			return highest;
		}

		public static double GetMinRewindableSeconds()
		{
			var count = GetMinRewindableSimulates();
			return SmoothDeltaTime * count;
		}

		public static double GetMaxRewindableSeconds()
		{
			return SmoothDeltaTime * MAX_TRACKED_FRAGMENTS;
		}
		
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new MinimalPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}

}
