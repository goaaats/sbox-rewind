using System;
using Sandbox;
/*

namespace rewind.Animation
{
	internal struct CitizenAnimationHelper
	{
		private readonly AnimEntity Owner;

		public CitizenAnimationHelper( AnimEntity entity )
		{
			this.Owner = entity;
		}

		public void WithLookAt( Vector3 look )
		{
			this.Owner.SetAnimLookAt( "aim_eyes", look );
			this.Owner.SetAnimLookAt( "aim_head", look );
			this.Owner.SetAnimLookAt( "aim_body", look );
			this.Owner.SetAnimFloat( "aimat_weight", 0.5f );
		}

		public void WithVelocity( Vector3 Velocity )
		{
			var dir = Velocity;
			var forward = this.Owner.Rotation.Forward.Dot( dir );
			var sideward = this.Owner.Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			this.Owner.SetAnimFloat( "move_direction", angle );
			this.Owner.SetAnimFloat( "move_speed", Velocity.Length );
			this.Owner.SetAnimFloat( "move_groundspeed", Velocity.WithZ( 0 ).Length );
			this.Owner.SetAnimFloat( "move_y", sideward );
			this.Owner.SetAnimFloat( "move_x", forward );
		}

		public void WithWishVelocity( Vector3 Velocity )
		{
			var dir = Velocity;
			var forward = this.Owner.Rotation.Forward.Dot( dir );
			var sideward = this.Owner.Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			this.Owner.SetAnimFloat( "wish_direction", angle );
			this.Owner.SetAnimFloat( "wish_speed", Velocity.Length );
			this.Owner.SetAnimFloat( "wish_groundspeed", Velocity.WithZ( 0 ).Length );
			this.Owner.SetAnimFloat( "wish_y", sideward );
			this.Owner.SetAnimFloat( "wish_x", forward );
		}
	}
}
*/
