using System.Collections.Generic;
using rewind.Fragment;

namespace rewind.Rewindable
{
	public interface IRewindable
	{
		public Stack<RewindFragment> Fragments { get; set; } 
		
		public void RewindTick();

		public void UpdateRewindState( RewindMode mode );
	}
}
