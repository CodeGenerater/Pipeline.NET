namespace System.Threading
{
	public interface IPipe : INode<IPipe>, IInitializable
	{
		void Receive(object Data);
	}
}