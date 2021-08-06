using System.Collections.Generic;
using rewind.Fragment;
using Sandbox;

namespace rewind.Player
{
	[Library("ent_rewind_ghost")]
	public class RewindGhost : AnimEntity
	{
		private Stack<RewindFragment> fragments { get; set; }
		private RewindFragment lastFragment { get; set; }
		
		public RewindGhost( Stack<RewindFragment> fragments, ModelEntity living )
		{
			this.fragments = fragments;
			
			SetModel("models/citizen/citizen.vmdl");
			
			//CopyMaterialGroup( living );
			//TakeDecalsFrom( living );
			//CopyBodyGroups( living );

			EnableAllCollisions = false;
			SetBonePhysics( false );
			
			Log.Info( "Creating ghost..." );
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
		
		public override void Spawn()
		{
			RenderAlpha = 0.4f;
			
			base.Spawn();
		}

		[Event.Tick.Client]
		public void Tick()
		{
			if ( this.fragments.TryPop( out var fragment ) )
			{
				this.ApplyFragment( fragment );

				this.lastFragment = fragment;
			}
			else
			{
				//this.ApplyFragment( this.lastFragment );
				this.Delete();
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
			
			fragment.RestoreAnimator( this );
		}
	}
}
