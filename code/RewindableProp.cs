using System;
using System.Collections.Generic;
using System.Linq;
using rewind;
using Sandbox;

namespace rewind
{
	[Library("ent_prop_rewindable")]
	public partial class RewindableProp : Prop, IRewindable
	{
		public Stack<RewindFragment> Fragments { get; set; } = new(RewindGame.MAX_TRACKED_FRAGMENTS + 1);

		private RewindFragment lastFragment;

		public void RewindSimulate()
		{
			//DebugOverlay.Text( EyePos, 0, $"Fragments: {Fragments.Count}", Color.White );

			if ( RewindGame.Mode == RewindMode.Gameplay )
			{
				Fragments.Push( new RewindFragment(this) );
				
				// TODO: Handle more fragments than MAX_FRAGMENT_COUNT
			}
			else if (RewindGame.Mode == RewindMode.Rewind)
			{
				if ( Fragments.TryPop( out var fragment ) )
				{
					ApplyFragment( fragment );

					this.lastFragment = fragment;
				}
				else
				{
					ApplyFragment( this.lastFragment );
				}
			}
		}

		public void UpdateRewindState( RewindMode mode )
		{
			switch ( mode )
			{
				case RewindMode.Gameplay:
					PhysicsEnabled = true;
					EnableAllCollisions = true;
					EnableTouch = true;
					break;
				case RewindMode.Rewind:
					PhysicsEnabled = false;
					EnableAllCollisions = false;
					EnableTouch = false;
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof(mode), mode, null );
			}
		}

		private void ApplyFragment( RewindFragment fragment )
		{
			Position = fragment.Position;
			EyePos = fragment.EyePos;
			Rotation = fragment.Rotation;
			Velocity = fragment.Velocity;
		}
	}
}
