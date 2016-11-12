namespace System.Threading
{
	public static class NodeExtensions
	{
		public static TNode Connect<TNode>(this TNode This, TNode Next) where TNode : INode<TNode>
		{
			This.Next = Next;
			Next.Prev = This;
			return Next;
		}

		public static TNode Insert<TNode>(this TNode This, TNode Next) where TNode : INode<TNode>
		{
			if (This.Next != null)
				This.Next.Prev = Next;
			Next.Next = This.Next;
			This.Next = Next;
			Next.Prev = This;
			return Next;
		}

		public static TNode Disconnect<TNode>(this TNode This) where TNode : INode<TNode>
		{
			if (This.Prev != null)
				This.Prev.Next = This.Next;
			if (This.Next != null)
				This.Next.Prev = This.Prev;
			return This.Next;
		}
	}
}