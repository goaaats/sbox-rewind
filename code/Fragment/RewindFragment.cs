using System.Collections.Generic;
using Sandbox;

namespace rewind.Fragment
{
	public partial struct RewindFragment
	{
		public Vector3 Position;
		public Vector3 EyePos;
		public Rotation Rotation;
		public Vector3 Velocity;
		public Vector3 LocalPosition;
		public Rotation LocalRotation;
		public Angles AngularVelocity;

		public Transform[] Bones;
		
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
