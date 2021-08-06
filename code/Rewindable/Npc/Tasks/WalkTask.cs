using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace rewind.Rewindable.Npc.Tasks
{
	public class WalkTask : BaseTask
	{
		public override string Name => "Walking";
		public override float Speed => 150;
		
		public List<Vector3> Points { get; set; }

		public WalkTask(RewindableNpc owner) : base(owner) {
			//Log.Info( $"WalkTask::ctor {NavMesh.IsLoaded}" );
			
			//int distMult = Rand.Int(0, 3);
			//Vector3? attemptedDest = RewindGame.GetPointWithinRadius(Owner.Position, 250 * distMult, 1000 * distMult);

			//if(attemptedDest is not Vector3 dest)
			//{
			//	Completed = true;
			//	return;
			//}

			//Vector3? closestPoint = RewindGame.GetClosestPoint(Owner.Position);
			//Vector3[] rawPoints = RewindGame.BuildPath(closestPoint.Value, dest);
			
			//if (rawPoints is null)
			//{
			//	Completed = true;
			//	return;
			//}

			//Points = rawPoints.ToList();
		}

		TimeSince stuckFor = 0;

		public override Vector3 CalculateInputVelocity()
		{
			//Log.Info( "WalkTask::CalculateInputVelocity()" );

			if ( Points == null )
				return Vector3.Zero;
			
			Vector3 currentTarget = Points[Points.Count - 1];

			if (Owner.Velocity.Length < 100)
			{
				if (stuckFor > 3)
					Completed = true;
				
			}
			else
				stuckFor = 0;

			if (Vector3.DistanceBetween(Owner.Position.WithZ(0), currentTarget.WithZ(0)) > 20f)
			{
				Vector3 direction = (currentTarget - (Owner.Position)).Normal.WithZ(0);
				Vector3 avoid = GetAvoidance(direction, Owner.Position, 500);
				return (direction + avoid).Normal;

			} else
			{
				Points.Remove(currentTarget);
				
				if (Points.Count == 0) 
					Owner.CurrentTask = new SleepTask(Owner, 5, 13);
			}

			return Vector3.Zero;
		}

		Vector3 GetAvoidance(Vector3 Direction, Vector3 position, float radius)
		{
			var center = position + Direction * radius * 0.5f;

			var objectRadius = 200.0f;
			Vector3 avoidance = default;

			foreach (var ent in Physics.GetEntitiesInSphere(center, radius))
			{
				if (ent is not RewindableNpc) continue;
				if (ent.IsWorld) continue;

				var delta = (position - ent.Position).WithZ(0);
				var closeness = delta.Length;
				if (closeness < 0.001f) continue;
				var thrust = ((objectRadius - closeness) / objectRadius).Clamp(0, 1);
				if (thrust <= 0) continue;

				//avoidance += delta.Cross( Output.Direction ).Normal * thrust * 2.5f;
				avoidance += delta.Normal * thrust * thrust;
			}

			return avoidance;
		}
	}
}
