using System.Collections.Generic;

namespace System.Threading
{
	public interface ILinkedList<TNode> : INode<TNode>, IList<TNode> where TNode : INode<TNode>
	{
	}
}