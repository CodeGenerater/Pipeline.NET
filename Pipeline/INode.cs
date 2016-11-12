namespace System.Threading
{
	public interface INode<TNode> where TNode : INode<TNode>
	{
		#region Property
		TNode Prev { set; get; }

		TNode Next { set; get; }
		#endregion
	}
}