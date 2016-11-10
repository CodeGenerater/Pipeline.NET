using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace System.Threading
{
	public class NodeList<TNode> : Node<TNode>, IList<TNode> where TNode : Node<TNode>
	{
		#region Field
		TNode Head = null;

		TNode Tail = null;
		#endregion

		#region Property
		public TNode this[int index]
		{
			get
			{
				if (index >= Count) throw new IndexOutOfRangeException();

				bool StartFromHead = CalcNearNode(index);

				TNode Node = StartFromHead ? Head : Tail;
				int Length = StartFromHead ? index : Count - index - 1;

				for (int i = 0; i < Length; i++)
					if (StartFromHead) Node = Node.Next;
					else Node = Node.Prev;

				return Node;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				TNode Node = this[index];
				Node.Prev.Connect(value);
				Node.Disconnect();
			}
		}

		public int Count
		{
			private set;
			get;
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Method
		public void Add(TNode tail)
		{
			if (tail == null)
				throw new ArgumentNullException();

			if (Count == 0)
				Head = tail;
			else
				Tail.Connect(tail);

			Tail = tail;

			Count++;
		}

		public void Clear()
		{
			TNode Node = Head;

			while (Node != null)
				Node = Node.Disconnect();

			Head = null;
			Tail = null;
			Count = 0;
		}

		public bool Contains(TNode item)
		{
			if (Count > 10)
				return Contains_MultiTask(item);
			else
				return Contains_SingleThread(item);
		}

		public void CopyTo(TNode[] array, int arrayIndex)
		{
			TNode Node = Head;

			for (int i = arrayIndex; i < array.Length && Node != null; i++, Node = Node.Next)
				array[i] = Node;
		}

		public int IndexOf(TNode item)
		{
			TNode Node = Head;
			int Index = 0;

			while (Node != null)
			{
				if (Node == item) return Index;

				Index++;
				Node = Node.Next;
			}

			return -1;
		}

		public void Insert(int index, TNode item)
		{
			if (Count == 0)
			{
				Head = Tail = item;
			}
			else if (index == 0)
			{
				item.Connect(Head);
				Head = item;
			}
			else
			{
				this[index - 1].Insert(item);
			}

			Count++;
		}

		public bool Remove(TNode item)
		{
			if (item == null)
				throw new ArgumentNullException();

			if (Contains(item))
			{
				_Remove(item);
				return true;
			}

			return false;
		}

		public void RemoveAt(int index)
		{
			if (index >= Count)
				throw new IndexOutOfRangeException();

			_Remove(this[index]);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			TNode Node = Head;

			while (Node != null)
			{
				yield return Node;
				Node = Node.Next;
			}
		}

		public IEnumerator<TNode> GetEnumerator()
		{
			TNode Node = Head;

			while (Node != null)
			{
				yield return Node;
				Node = Node.Next;
			}
		}
		#endregion

		#region Helper
		bool CalcNearNode(int Index)
		{
			return Count - Index > Count / 2;
		}

		bool Contains_SingleThread(TNode item)
		{
			TNode Node = Head;

			while (Node != null)
			{
				if (Node == item) return true;
				else Node = Node.Next;
			}

			return false;
		}

		bool Contains_MultiTask(TNode item)
		{
			bool Result = false;
			int Length = Count / 2;

			Task StartFromHead = new Task(() =>
			{
				TNode Node = Head;

				for (int i = 0; i < Length && Node != null; i++, Node = Node.Next)
					if (Result) return;
					else if (Node == item)
					{
						Result = true;
						return;
					}
			});

			Task StartFromTail = new Task(() =>
			{
				TNode Node = Tail;
				int length = Count - Length;

				for (int i = 0; i < length && Node != null; i++, Node = Node.Prev)
					if (Result) return;
					else if (Node == item)
					{
						Result = true;
						return;
					}
			});

			StartFromHead.Start();
			StartFromTail.Start();

			Task.WaitAll(StartFromHead, StartFromTail);

			return Result;
		}

		void _Remove(TNode item)
		{
			if (item == Head)
				Head = item.Next;
			if (item == Tail)
				Tail = item.Prev;
			Count--;
			item.Disconnect();
		}
		#endregion
	}
}