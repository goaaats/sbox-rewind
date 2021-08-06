namespace rewind.Rewindable.Npc.Tasks
{
	public class RunTask : WalkTask
	{
		public RunTask(RewindableNpc owner) : base(owner)
		{

		}
		
		public override string Name => "Running";
		public override float Speed => 300;
	}
}
