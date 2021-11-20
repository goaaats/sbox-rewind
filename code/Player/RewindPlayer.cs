using System;
using System.Collections.Generic;
using System.Linq;
using rewind.Fragment;
using rewind.Rewindable;
using Sandbox;

namespace rewind.Player
{
	internal partial class RewindPlayer : Sandbox.Player, IRewindable
	{
		private Clothing.Container clothing = new();

		public RewindPlayer() { }

		public RewindPlayer( Client cl )
		{
			clothing.LoadFromClient( cl );
		}

		public override void Respawn()
		{
			this.SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new RewindCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			clothing.DressEntity( this );

			base.Respawn();
		}

		private ModelEntity pants;
		private ModelEntity jacket;
		private ModelEntity shoes;
		private ModelEntity hat;

		private bool dressed;

		/// <summary>
		///     Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			
			this.SimulateActiveChild( cl, ActiveChild );
			
			if ( RewindGame.Mode == RewindMode.Gameplay )
			{
				if ( IsClient && Input.Down( InputButton.Attack1 ) )
				{
					var ent = new RewindableProp {Position = EyePos + EyeRot.Forward * 50, Rotation = EyeRot};

					ent.SetModel( "models/citizen_props/crate01.vmdl" );
					ent.Velocity = EyeRot.Forward * 1000;
				}
				
				if ( IsClient && Input.Pressed( InputButton.Slot1 ) )
				{
					RewindGame.SpawnDebugNpcs( 5 );
				}

				if ( IsClient && Input.Pressed( InputButton.Flashlight ) )
				{
					var ragdoll = new RewindableModelEntity();
					ragdoll.SetModel( "models/citizen/citizen.vmdl" );
					ragdoll.Position = EyePos + EyeRot.Forward * 40;
					ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
					ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
					ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
				}
			}

			if ( IsClient && Input.Pressed( InputButton.Attack2 ) )
			{
				RewindGame.Mode = RewindMode.Rewind;
				Log.Info( "Rewinding..." );
			}
			else if ( IsClient && Input.Released( InputButton.Attack2 ) )
			{
				RewindGame.Mode = RewindMode.Gameplay;
				Log.Info( "Gameplay..." );
			}

			if ( IsServer && Input.Pressed( InputButton.Use ) )
			{
				this.DeleteRewindables();
			}
			
			if ( IsClient && Input.Pressed( InputButton.Use ) )
			{
				this.DeleteRewindables();
			}
		}
		
		private void DeleteRewindables()
		{
			foreach ( var entity in All.Where( x => x is IRewindable and not RewindPlayer ) )
			{
				entity.Delete();
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}

		public Stack<RewindFragment> Fragments { get; set; } = new();
		public void RewindTick()
		{
			if ( RewindGame.Mode != RewindMode.Gameplay )
			{
				return;
			}

			var fragment = new RewindFragment( this );
			fragment.SaveBones( this );
			fragment.SaveAnimator( this );
			
			Fragments.Push( fragment );
		}

		public void UpdateRewindState( RewindMode mode )
		{
			switch ( mode )
			{
				case RewindMode.Gameplay:
					Fragments.Clear();
					break;
				case RewindMode.Rewind:
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof(mode), mode, null );
			}
		}
	}
}
