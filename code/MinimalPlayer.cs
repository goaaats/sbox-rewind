using System.Linq;
using Sandbox;

namespace rewind
{
	internal class MinimalPlayer : Player
	{
		public override void Respawn()
		{
			this.SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			this.Dress();

			base.Respawn();
		}

		private ModelEntity pants;
		private ModelEntity jacket;
		private ModelEntity shoes;
		private ModelEntity hat;

		private bool dressed;

		public void Dress()
		{
			if ( this.dressed )
			{
				return;
			}

			this.dressed = true;

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
					"models/citizen_clothes/trousers/trousers.jeans.vmdl",
					"models/citizen_clothes/trousers/trousers.lab.vmdl"
				} );

				this.pants = new ModelEntity();
				this.pants.SetModel( model );
				this.pants.SetParent( this, true );
				this.pants.EnableShadowInFirstPerson = true;
				this.pants.EnableHideInFirstPerson = true;

				this.SetBodyGroup( "Legs", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[] {"models/citizen_clothes/jacket/labcoat.vmdl"} );

				this.jacket = new ModelEntity();
				this.jacket.SetModel( model );
				this.jacket.SetParent( this, true );
				this.jacket.EnableShadowInFirstPerson = true;
				this.jacket.EnableHideInFirstPerson = true;

				var propInfo = this.jacket.GetModel().GetPropData();
				if ( propInfo.ParentBodyGroupName != null )
				{
					this.SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );
				}
				else
				{
					this.SetBodyGroup( "Chest", 0 );
				}
			}

			if ( true )
			{
				var model = Rand.FromArray( new[] {"models/citizen_clothes/shoes/trainers.vmdl"} );

				this.shoes = new ModelEntity();
				this.shoes.SetModel( model );
				this.shoes.SetParent( this, true );
				this.shoes.EnableShadowInFirstPerson = true;
				this.shoes.EnableHideInFirstPerson = true;

				this.SetBodyGroup( "Feet", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
					"models/citizen_clothes/hat/hat_hardhat.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
					"models/citizen_clothes/hair/hair_malestyle02.vmdl",
					"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
					"models/citizen_clothes/hat/hat_woollybobble.vmdl"
				} );

				this.hat = new ModelEntity();
				this.hat.SetModel( model );
				this.hat.SetParent( this, true );
				this.hat.EnableShadowInFirstPerson = true;
				this.hat.EnableHideInFirstPerson = true;
			}
		}

		/// <summary>
		///     Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			this.SimulateActiveChild( cl, ActiveChild );

			//
			// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
			//
			if ( IsClient && Input.Down( InputButton.Attack1 ) )
			{
				var ent = new RewindableProp {Position = EyePos + EyeRot.Forward * 50, Rotation = EyeRot};

				ent.SetModel( "models/citizen_props/crate01.vmdl" );
				ent.Velocity = EyeRot.Forward * 1000;
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

			if ( IsClient && Input.Pressed( InputButton.Use ) )
			{
				foreach ( var entity in All.Where( x => x is IRewindable ) )
				{
					entity.Delete();
				}
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
