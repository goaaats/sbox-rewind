namespace rewind.Rewindable.Npc.Tasks
{
	public abstract class BaseTask
	{
		public abstract float Speed { get; }
		public abstract string Name { get; }

		public bool Completed { get; set; } = false;
		public RewindableNpc Owner { get; set; }


		public BaseTask(RewindableNpc owner) => Owner = owner;
		public abstract Vector3 CalculateInputVelocity();
	}
}
