using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;
using rewind.Player;
using rewind.Rewindable;
using rewind.Rewindable.Npc;
using rewind.UI;

//#define INDEV

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

			if ( IsClient )
			{
				_ = StartSecondTimer();
			}
		}

		public async Task StartSecondTimer()
		{
			while (true)
			{
				await Task.DelaySeconds( 0.3f );
				OnSecond();
			}
		}

		private void OnSecond()
		{
			if ( IsClient )
			{
				foreach ( var npc in All.OfType<RewindableNpc>() )
				{
					RequestPath( npc.NetworkIdent, npc.Position, Local.Pawn.Position );
				}
			}
		}

		private static void NotifyStateChange( RewindMode mode )
		{
			if ( mode == RewindMode.Rewind )
			{
				rewindingSince = DateTimeOffset.Now;

				var player = Local.Pawn as RewindPlayer;
				new RewindGhost( player.Fragments.Clone(), player );
			}
			else
			{
				ConsoleSystem.Run( "host_timescale", 1.0f );
				
				Log.Info( "Deleting Ghosts..." );
				foreach (var rewindGhost in All.OfType<RewindGhost>())
				{
					rewindGhost.MarkDelete();
				}
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

#if INDEV
			DebugOverlay.ScreenText( 6, "MinCnt: " + GetMinRewindableSimulates() );
			DebugOverlay.ScreenText( 7, $"MinSec: {GetMinRewindableSeconds():00.0} / {GetMaxRewindableSeconds():00.0} " );
			DebugOverlay.ScreenText( 8, $"SmoothDeltaTime: {SmoothDeltaTime}" );
			DebugOverlay.ScreenText( 9, $"RewindTimescale: {RewindTimescale}" );

#endif
			
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
