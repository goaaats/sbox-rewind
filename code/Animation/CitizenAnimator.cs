using Sandbox;

namespace rewind.Animation
{
	public class CitizenAnimator
	{
		private AnimEntity Owner { get; }

		private CitizenAnimationHelper animHelper;
		private Vector3 lookDir;
		private float duck;

		public CitizenAnimator( AnimEntity ent )
		{
			this.animHelper = new CitizenAnimationHelper( ent );
			Owner = ent;
		}

		public void Tick( Vector3 wishVelocity, Vector3 velocity, bool doCrouch = false )
		{
			if ( wishVelocity.WithZ( 0 ).Length > .5f )
			{
				var targetRotation = Rotation.LookAt( wishVelocity.Normal, Vector3.Up );
				Owner.Rotation = Rotation.Lerp( Owner.Rotation, targetRotation, Time.Delta * 20 );
			}

			this.lookDir = Vector3.Lerp( this.lookDir, velocity.WithZ( 0 ) * 1000, Time.Delta * 100 );
			this.animHelper.WithLookAt( Owner.EyePos + this.lookDir );

			this.animHelper.WithWishVelocity( wishVelocity );
			this.animHelper.WithVelocity( velocity );


			//Is the player on the ground
			Owner.SetAnimBool( "b_grounded", Owner.GroundEntity is not null );


			//Handle the duck lerping
			if ( doCrouch )
			{
				this.duck = this.duck.LerpTo( 1, Time.Delta * 10 );
			}
			else
			{
				this.duck = this.duck.LerpTo( 0, Time.Delta * 5 );
			}

			Owner.SetAnimBool( "b_ducked", doCrouch );
			Owner.SetAnimFloat( "duck", this.duck );
		}
	}
}
