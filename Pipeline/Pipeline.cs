using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace System.Threading
{
	public class Pipeline : ILinkedList<IPipe>, IPipe
	{
		#region Constructor
		public Pipeline()
		{
			Initialize();
		}

		public Pipeline(IEnumerable<IPipe> PipeCollection) : this()
		{
			foreach (var each in PipeCollection)
				Add(each);
		}
		#endregion

		#region Finalizer
		~Pipeline()
		{
			Dispose();
		}
		#endregion

		#region Field
		IPipe Head = null;

		IPipe Tail = null;

		IPipe _Prev;

		IPipe _Next;
		#endregion

		#region Property
		public IPipe this[int index]
		{
			get
			{
				if (index >= Count) throw new IndexOutOfRangeException();

				bool StartFromHead = CalcNearNode(index);

				IPipe Node = StartFromHead ? Head : Tail;
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

				IPipe Node = this[index];
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

		public IPipe Prev
		{
			get
			{
				return _Prev;
			}

			set
			{
				_Prev = value;
			}
		}

		public IPipe Next
		{
			get
			{
				return _Next;
			}

			set
			{
				_Next = value;
				this.Last().Next = value;
			}
		}
		#endregion

		#region Method
		#region IList
		public void Add(IPipe tail)
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
			IPipe Node = Head;

			while (Node != null)
				Node = Node.Disconnect();

			Head = null;
			Tail = null;
			Count = 0;
		}

		public bool Contains(IPipe item)
		{
			if (Count > 10)
				return Contains_MultiTask(item);
			else
				return Contains_SingleThread(item);
		}

		public void CopyTo(IPipe[] array, int arrayIndex)
		{
			IPipe Node = Head;

			for (int i = arrayIndex; i < array.Length && Node != null; i++, Node = Node.Next)
				array[i] = Node;
		}

		public int IndexOf(IPipe item)
		{
			IPipe Node = Head;
			int Index = 0;

			while (Node != null)
			{
				if (Node == item) return Index;

				Index++;
				Node = Node.Next;
			}

			return -1;
		}

		public void Insert(int index, IPipe item)
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

		public bool Remove(IPipe item)
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
			IPipe Node = Head;

			while (Node != null)
			{
				yield return Node;
				Node = Node.Next;
			}
		}

		public IEnumerator<IPipe> GetEnumerator()
		{
			IPipe Node = Head;

			while (Node != null)
			{
				yield return Node;
				Node = Node.Next;
			}
		}
		#endregion

		public void Initialize()
		{

		}

		public void Dispose()
		{
			for (IPipe Pipe = Head; Pipe != null; Pipe = Pipe.Next)
				Pipe?.Dispose();

			Head = null;
			Tail = null;
		}
		#endregion

		#region Helper
		bool CalcNearNode(int Index)
		{
			return Count - Index > Count / 2;
		}

		bool Contains_SingleThread(IPipe item)
		{
			IPipe Node = Head;

			while (Node != null)
			{
				if (Node == item) return true;
				else Node = Node.Next;
			}

			return false;
		}

		bool Contains_MultiTask(IPipe item)
		{
			bool Result = false;
			int Length = Count / 2;

			Task StartFromHead = new Task(() =>
			{
				IPipe Node = Head;

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
				IPipe Node = Tail;
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

		void _Remove(IPipe item)
		{
			if (item == Head)
				Head = item.Next;
			if (item == Tail)
				Tail = item.Prev;
			Count--;
			item.Disconnect();
		}

		public void Receive(object Data)
		{
			this.First()?.Receive(Data);
		}
		#endregion
	}
}