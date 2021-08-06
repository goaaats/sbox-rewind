using System.Collections.Generic;
using Sandbox;

namespace rewind.Fragment
{
	public struct RewindFragment
	{
		public Vector3 Position;
		public Vector3 EyePos;
		public Rotation Rotation;
		public Vector3 Velocity;
		public Vector3 LocalPosition;
		public Rotation LocalRotation;
		public Angles AngularVelocity;

		public Transform[] Bones;
		
		// bools
		public bool Ducked;
		public bool Grounded;
		
		// floats
		public float Duck;
		public float AimAtWeight;
		public float MoveDirection;
		public float MoveSpeed;
		public float MoveGroundSpeed;
		public float MoveY;
		public float MoveX;
		public float WishDirection;
		public float WishSpeed;
		public float WishGroundSpeed;
		public float WishY;
		public float WishX;
		
		// lookat
		public Vector3 AimEyes;
		public Vector3 AimHead;
		public Vector3 AimBody;

		public string Sequence;

		public RewindFragment( Entity e )
		{
			this.Position = e.Position;
			this.EyePos = e.EyePos;
			this.Rotation = e.Rotation;
			this.Velocity = e.Velocity;
			this.AngularVelocity = e.AngularVelocity;
			this.LocalPosition = e.LocalPosition;
			this.LocalRotation = e.LocalRotation;

			this.Bones = null;
			
			this.Ducked = false;
			this.Grounded = false;
			this.Duck = 0;
			this.AimAtWeight = 0;
			this.MoveDirection = 0;
			this.MoveSpeed = 0;
			this.MoveGroundSpeed = 0;
			this.MoveY = 0;
			this.MoveX = 0;
			this.WishDirection = 0;
			this.WishSpeed = 0;
			this.WishGroundSpeed = 0;
			this.WishY = 0;
			this.WishX = 0;
			this.AimEyes = default;
			this.AimHead = default;
			this.AimBody = default;
			this.Sequence = null;
		}
		
		public void SaveAnimator( AnimEntity entity )
		{
			this.Sequence = entity.Sequence;

			this.Ducked = entity.GetAnimBool( "b_ducked" );
			this.Grounded = entity.GetAnimBool( "b_grounded" );

			this.Duck = entity.GetAnimFloat( "duck" );
			this.AimAtWeight = entity.GetAnimFloat( "aimat_weight" );
			this.MoveDirection = entity.GetAnimFloat( "move_direction" );
			this.MoveSpeed = entity.GetAnimFloat( "move_speed" );
			this.MoveGroundSpeed = entity.GetAnimFloat( "move_groundspeed" );
			this.MoveY = entity.GetAnimFloat( "move_y" );
			this.MoveX = entity.GetAnimFloat( "move_x" );
			this.WishDirection = entity.GetAnimFloat( "wish_direction" );
			this.WishSpeed = entity.GetAnimFloat( "wish_speed" );
			this.WishGroundSpeed = entity.GetAnimFloat( "wish_groundspeed" );
			this.WishY = entity.GetAnimFloat( "wish_y" );
			this.WishX = entity.GetAnimFloat( "wish_x" );
			
			this.AimEyes = entity.GetAnimVector( "aim_eyes" );
			this.AimHead = entity.GetAnimVector( "aim_head" );
			this.AimBody = entity.GetAnimVector( "aim_body" );
		}

		public void RestoreAnimator( AnimEntity entity )
		{
			entity.SetAnimBool( "b_ducked", this.Ducked );
			entity.SetAnimBool( "b_grounded", this.Grounded );
			
			entity.SetAnimFloat( "duck", this.Duck );
			entity.SetAnimFloat( "aimat_weight", this.AimAtWeight );
			entity.SetAnimFloat( "move_direction", this.MoveDirection );
			entity.SetAnimFloat( "move_speed", this.MoveSpeed );
			entity.SetAnimFloat( "move_groundspeed", this.MoveGroundSpeed );
			entity.SetAnimFloat( "move_y", this.MoveY );
			entity.SetAnimFloat( "move_x", this.MoveX );
			entity.SetAnimFloat( "wish_direction", this.WishDirection );
			entity.SetAnimFloat( "wish_speed", this.WishSpeed );
			entity.SetAnimFloat( "wish_groundspeed", this.WishGroundSpeed );
			entity.SetAnimFloat( "wish_y", this.WishY );
			entity.SetAnimFloat( "wish_x", this.WishX );
			
			entity.SetAnimLookAt( "aim_eyes", this.AimEyes );
			entity.SetAnimLookAt( "aim_head", this.AimHead );
			entity.SetAnimLookAt( "aim_body", this.AimBody );
		}

		public void SaveBones( ModelEntity e )
		{
			this.Bones = new Transform[e.BoneCount];

			for ( var i = 0; i < e.BoneCount; i++ )
			{
				this.Bones[i] = e.GetBoneTransform( i );
			}
		}
	}
}
