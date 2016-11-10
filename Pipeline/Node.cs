namespace System.Threading
{
	public class Node<TNode> where TNode : Node<TNode>
	{
		#region Property
		public TNode Prev
		{
			set;
			get;
		}

		public virtual TNode Next
		{
			set;
			get;
		}
		#endregion

		#region Method
		public TNode Connect(TNode Next)
		{
			this.Next = Next;
			Next.Prev = (TNode)this;
			return Next;
		}

		public TNode Insert(TNode Next)
		{
			if (this.Next != null)
				this.Next.Prev = Next;
			Next.Next = this.Next;
			this.Next = Next;
			Next.Prev = (TNode)this;
			return Next;
		}

		public TNode Disconnect()
		{
			if (Prev != null)
				Prev.Next = Next;
			if (Next != null)
				Next.Prev = Prev;
			return Next;
		}
		#endregion
	}
}