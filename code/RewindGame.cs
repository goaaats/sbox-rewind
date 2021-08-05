using Sandbox;
using System;
using System.Linq;
using rewind.Player;
using rewind.Rewindable;
using rewind.UI;

namespace rewind
{
	[Library]
	public partial class RewindGame : Sandbox.Game
	{
		public const int MAX_TRACKED_FRAGMENTS = 5000;
		
		private const float MAX_SECONDS_UNTIL_FULL_SPEED = 1.3f;
		private const float MIN_TIMESCALE = 0.1f;
		private const float MAX_TIMESCALE = 1.1f;

		private static RewindMode modeInternal = RewindMode.Gameplay;

		private static DateTimeOffset rewindingSince;

		public static float RewindTimescale =>
			Math.Min( MIN_TIMESCALE + Math.Min( (float)(DateTimeOffset.Now - rewindingSince).TotalSeconds,
					MAX_SECONDS_UNTIL_FULL_SPEED ) /
				MAX_SECONDS_UNTIL_FULL_SPEED, MAX_TIMESCALE );

		public static RewindMode Mode
		{
			get => modeInternal;
			set
			{
				modeInternal = value;
				NotifyStateChange( value );
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
				new RewindHudEntity();
			}
		}

		private static void NotifyStateChange( RewindMode mode )
		{
			if ( mode == RewindMode.Rewind )
			{
				rewindingSince = DateTimeOffset.Now;
			}
			else
			{
				ConsoleSystem.Run( "host_timescale", 1.0f );
			}

			foreach ( var entity in All.OfType<IRewindable>() )
			{
				entity.UpdateRewindState( mode );
			}
		}

		[Event( "tick" )]
		private void Tick()
		{
			if ( IsClient )
			{
				foreach ( var entity in All.OfType<IRewindable>() )
				{
					entity.RewindTick();
				}
			}
		}

		public override void FrameSimulate( Client cl )
		{
			prevDeltaTimes[curTrackedIndex] = Time.Delta;
			curTrackedIndex++;
			if ( curTrackedIndex > TrackedDeltaTimes - 1 )
			{
				curTrackedIndex = 0;
			}

			DebugOverlay.ScreenText( 6, "MinCnt: " + GetMinRewindableSimulates() );
			DebugOverlay.ScreenText( 7, $"MinSec: {GetMinRewindableSeconds():00.0} / {GetMaxRewindableSeconds():00.0} " );
			DebugOverlay.ScreenText( 8, $"SmoothDeltaTime: {SmoothDeltaTime}" );
			DebugOverlay.ScreenText( 9, $"RewindTimescale: {RewindTimescale}" );

			if ( Mode == RewindMode.Rewind )
			{
				ConsoleSystem.Run( "host_timescale", RewindTimescale );
			}

			base.FrameSimulate( cl );
		}

		public static int GetMinRewindableSimulates()
		{
			var highest = 0;

			foreach ( var entity in All.OfType<IRewindable>() )
			{
				// Let's ignore entities with more fragments than our max for now, until we can sort this out
				if ( entity.Fragments.Count > MAX_TRACKED_FRAGMENTS )
				{
					continue;
				}

				if ( entity.Fragments.Count > highest )
				{
					highest = entity.Fragments.Count;
				}
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

			var player = new RewindPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}

}
