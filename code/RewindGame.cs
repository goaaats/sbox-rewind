
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using rewind;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace rewind
{
	[Library]
	public partial class RewindGame : Sandbox.Game
	{
		public const int MAX_TRACKED_FRAGMENTS = 5000;
		public static RewindMode Mode { get; set; } = RewindMode.Gameplay;


		private const int TrackedDeltaTimes = 100;
		private static float[] prevDeltaTimes = new float[TrackedDeltaTimes];
		private static  int curTrackedIndex = 0;

		public static float SmoothDeltaTime => prevDeltaTimes.Aggregate( ( x, y ) => x + y ) / TrackedDeltaTimes;

		public RewindGame()
		{
			if ( IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );

				// Create a HUD entity. This entity is globally networked
				// and when it is created clientside it creates the actual
				// UI panels. You don't have to create your HUD via an entity,
				// this just feels like a nice neat way to do it.
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
				entity.UpdateRewindState();
			}

			prevDeltaTimes[curTrackedIndex] = Time.Delta;
			curTrackedIndex++;
			if ( curTrackedIndex > TrackedDeltaTimes - 1 )
				curTrackedIndex = 0;
			
			DebugOverlay.ScreenText( 0, "MinCnt: " + GetMinRewindableSimulates() );
			DebugOverlay.ScreenText( 1, $"MinSec: {GetMinRewindableSeconds():00.0} / {GetMaxRewindableSeconds():00.0} " );
			DebugOverlay.ScreenText( 2, $"SmoothDeltaTime: {SmoothDeltaTime}" );

			base.FrameSimulate( cl );
		}

		public static int GetMinRewindableSimulates()
		{
			var highest = 0;
			
			foreach (var entity in RewindableProp.All.Where( x => x is IRewindable ).Cast<IRewindable>())
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

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new MinimalPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}

}
