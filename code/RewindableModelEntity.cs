using System;
using System.Collections.Generic;
using System.Linq;
using rewind;
using Sandbox;

namespace rewind
{
	[Library("ent_model_rewindable")]
	public partial class RewindableModelEntity : ModelEntity, IRewindable
	{
		private RewindFragment lastFragment;

		public Stack<RewindFragment> Fragments { get; set; } = new(RewindGame.MAX_TRACKED_FRAGMENTS + 1);

		public void RewindTick()
		{
			var debugPos = GetBoneTransform( 0 ).Position;
			DebugOverlay.Text( debugPos, 0, $"Fragments: {Fragments.Count}", Color.White );
			DebugOverlay.Text( debugPos, 2, $"Physics: {PhysicsEnabled}", Color.White );

			if ( RewindGame.Mode == RewindMode.Gameplay )
			{
				var frag = new RewindFragment( this );
				frag.SaveBones( this );
				
				Fragments.Push( frag );
				
				// TODO: Handle more fragments than MAX_FRAGMENT_COUNT
				
				DebugOverlay.Text( debugPos, 1, "Gameplay", Color.Gray );
			}
			else if (RewindGame.Mode == RewindMode.Rewind)
			{
				DebugOverlay.Text( debugPos, 1, "Rewind", Color.Red );

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
			switch (mode)
			{
				case RewindMode.Gameplay:
					//PhysicsEnabled = true;
					break;
				case RewindMode.Rewind:
					//PhysicsEnabled = false;
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
			
			for (var i = 0; i < fragment.Bones.Length; i++)
			{
				SetBoneTransform( i, fragment.Bones[i] );
			}
		}
	}
}
