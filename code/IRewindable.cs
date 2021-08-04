using System.Collections.Generic;

namespace rewind
{
	public interface IRewindable
	{
		public Stack<RewindFragment> Fragments { get; set; } 
		
		public void RewindSimulate();

		public void UpdateRewindState( RewindMode mode );
	}
}
