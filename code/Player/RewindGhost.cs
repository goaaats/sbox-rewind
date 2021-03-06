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

			this.CopyBonesFrom( living );
			this.CopyBodyGroups( living );
			this.CopyMaterialGroup( living );
			this.TakeDecalsFrom( living );

			EnableAllCollisions = false;
			this.SetBonePhysics( false );

			foreach ( var child in living.Children )
			{
				if ( !child.Tags.Has( "clothes" ) ) continue;
				if ( child is not ModelEntity e ) continue;

				var model = e.GetModelName();

				var clothing = new ModelEntity();
				clothing.SetModel( model );
				clothing.SetParent( this, true );
				clothing.RenderColor = e.RenderColor;
				clothing.CopyBodyGroups( e );
				clothing.CopyMaterialGroup( e );
			}

			Log.Info( "Creating ghost..." );
			
			SetAlpha( DefaultAlpha );
		}

		private void SetAlpha( float alpha )
		{
			RenderColor = Color.White.WithAlpha( alpha );
			
			foreach (var ent in Children.OfType<ModelEntity>())
			{
				ent.RenderColor = Color.White.WithAlpha( alpha );
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

		public void MarkDelete()
		{
			if ( !this.canDie )
			{
				this.canDie = true;
				this.timeSinceCanDie = 0;
			}
		}

		[Event.Tick.Client]
		public void Tick()
		{
			if ( this.canDie )
			{
				this.ApplyFragment( this.lastFragment );

				var alpha = Math.Clamp(1 - ( this.timeSinceCanDie / 1.0f ), 0, DefaultAlpha);

				if ( RenderAlpha == alpha )
				{
					return;
				}

				this.SetAlpha( alpha );

				if ( alpha <= 0 )
				{
					this.Delete();
				}

				return;
			}
			
			if ( this.fragments.TryPop( out var fragment ) )
			{
				this.ApplyFragment( fragment );

				this.lastFragment = fragment;
			}
			else if (!this.canDie)
			{
				this.MarkDelete();
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

			if ( fragment.Bones != null )
			{
				for (var i = 0; i < fragment.Bones.Length; i++)
				{
					SetBoneTransform( i, fragment.Bones[i] );
				}
			}
			
			fragment.RestoreAnimator( this );
		}
	}
}
