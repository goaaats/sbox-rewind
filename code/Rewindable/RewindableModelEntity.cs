using System;
using System.Collections.Generic;
using System.Linq;
using rewind;
using rewind.Fragment;
using Sandbox;

namespace rewind.Rewindable
{
	[Library("ent_model_rewindable")]
	public partial class RewindableModelEntity : ModelEntity, IRewindable
	{
		private RewindFragment lastFragment;

		public Stack<RewindFragment> Fragments { get; set; } = new(RewindGame.MAX_TRACKED_FRAGMENTS + 1);

		public void RewindTick()
		{
			var debugPos = this.GetBoneTransform( 0 ).Position;
			//DebugOverlay.Text( debugPos, 0, $"Fragments: {Fragments.Count}", Color.White );

			if ( RewindGame.Mode == RewindMode.Gameplay )
			{
				var frag = new RewindFragment( this );
				frag.SaveBones( this );

				Fragments.Push( frag );

				// TODO: Handle more fragments than MAX_FRAGMENT_COUNT
			}
			else if ( RewindGame.Mode == RewindMode.Rewind )
			{
				if ( Fragments.TryPop( out var fragment ) )
				{
					this.ApplyFragment( fragment );

					this.lastFragment = fragment;
				}
				else
				{
					this.ApplyFragment( this.lastFragment );
				}
			}
		}

		public void UpdateRewindState( RewindMode mode )
		{
			switch (mode)
			{
				case RewindMode.Gameplay:
					SetPhysics( true );
					break;
				case RewindMode.Rewind:
					SetPhysics( false );
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof(mode), mode, null );
			}
		}

		private void SetPhysics( bool state )
		{
			EnableAllCollisions = state;
			SetBonePhysics( state );
		}

		private void SetBonePhysics(bool state)
		{
			for ( var i = 0; i < BoneCount; i++ )
			{
				var bp = GetBonePhysicsBody( i );
				if (bp == null)
					continue;
				
				// Wholly disabling the bone makes it disappear visually. No clue.
				bp.CollisionEnabled = state;
				bp.DragEnabled = state;
				bp.GravityEnabled = state;
			}
		}

		private void ApplyFragment( RewindFragment fragment )
		{
			Position = fragment.Position;
			EyePos = fragment.EyePos;
			Rotation = fragment.Rotation;
			Velocity = fragment.Velocity;
			LocalPosition = fragment.LocalPosition;
			LocalRotation = fragment.LocalRotation;
			
			for (var i = 0; i < fragment.Bones.Length; i++)
			{
				SetBoneTransform( i, fragment.Bones[i] );
			}
		}
	}
}
