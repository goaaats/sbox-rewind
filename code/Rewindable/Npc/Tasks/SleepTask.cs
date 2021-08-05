using Sandbox;

namespace rewind.Rewindable.Npc.Tasks
{
	public class SleepTask : BaseTask
	{
		public override string Name => "Sleeping";
		public override float Speed => 0;

		private TimeSince sleep = 0;
		private float sleepFor;

		public SleepTask(RewindableNpc owner, int min, int max) : base(owner)
		{
			sleepFor = Rand.Int(min, max);
		}

		public override Vector3 CalculateInputVelocity()
		{
			if ( sleepFor < sleep )
				Completed = true;

			return Vector3.Zero;
		}
	}
}
