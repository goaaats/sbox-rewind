using System;
using System.Collections.Generic;
using System.Linq;
using rewind.Fragment;
using Sandbox;

namespace rewind.Player
{
	[Library("ent_rewind_ghost")]
	public class RewindGhost : AnimEntity
	{
		private const float DefaultAlpha = 0.55f;

		private Stack<RewindFragment> fragments;
		private RewindFragment lastFragment;
		private bool canDie;
		private TimeSince timeSinceCanDie = 0;
		
		public RewindGhost( Stack<RewindFragment> fragments, ModelEntity living )
		{
			this.fragments = fragments;

			this.SetModel( "models/citizen/citizen.vmdl" );

			Scale = living.Scale;

			this.CopyMaterialGroup( living );
			this.TakeDecalsFrom( living );
			this.CopyBodyGroups( living );

			EnableAllCollisions = false;
			this.SetBonePhysics( false );

			foreach ( ModelEntity clothing in living.Children )
			{
				if ( !clothing.Tags.Has( "clothing" ) )
				{
					return;
				}

				ModelEntity nClothing = new(clothing.GetModel().Name, this);
				nClothing.CopyMaterialGroup( clothing );
			}

			Log.Info( "Creating ghost..." );
			
			SetAlpha( DefaultAlpha );
		}

		private void SetAlpha( float alpha )
		{
			RenderAlpha = alpha;
			
			foreach (var ent in Children.OfType<ModelEntity>())
			{
				ent.RenderAlpha = alpha;
			}
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

		[Event.Tick.Client]
		public void Tick()
		{
			if ( this.fragments.TryPop( out var fragment ) )
			{
				this.ApplyFragment( fragment );

				this.lastFragment = fragment;
			}
			else if (!this.canDie)
			{
				this.canDie = true;
				this.timeSinceCanDie = 0;
			}
			else if (this.canDie)
			{
				this.ApplyFragment( this.lastFragment );
				
				var alpha = Math.Clamp(1 - Math.Clamp( this.timeSinceCanDie - 2, 0, 1 ), 0, DefaultAlpha);
				
				DebugOverlay.Text( Position, alpha.ToString() );

				if ( RenderAlpha == alpha )
				{
					return;
				}

				this.SetAlpha( alpha );

				if ( alpha == 0 )
				{
					this.Delete();
				}
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
