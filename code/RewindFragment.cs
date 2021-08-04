using System.Collections.Generic;
using Sandbox;

namespace rewind
{
	public struct RewindFragment
	{
		public Vector3 Position { get; set; }
		public Vector3 EyePos { get; set; }
		public Rotation Rotation { get; set; }
		public Vector3 Velocity { get; set; }
		public Vector3 LocalPosition { get; set; }
		public Rotation LocalRotation { get; set; }
		public Angles AngularVelocity { get; set; }
		
		public Transform[] Bones { get; set; }
		
		public RewindFragment( Entity e )
		{
			Position = e.Position;
			EyePos = e.EyePos;
			Rotation = e.Rotation;
			Velocity = e.Velocity;
			AngularVelocity = e.AngularVelocity;
			LocalPosition = e.LocalPosition;
			LocalRotation = e.LocalRotation;

			Bones = null;
		}

		public void SaveBones(ModelEntity e)
		{
			Bones = new Transform[e.BoneCount];
			
			for ( var i = 0; i < e.BoneCount; i++ )
			{
				Bones[i] = e.GetBoneTransform( i );
			}
		}
	}
}
