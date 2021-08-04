using System.Collections.Generic;

namespace rewind
{
	public class FIFOStack<T> : LinkedList<T>
	{
		public T Pop()
		{
			var first = First.Value;
			RemoveFirst();
			return first;
		}

		public void Push(T obj)
		{
			AddFirst(obj);
		}
	}
}
