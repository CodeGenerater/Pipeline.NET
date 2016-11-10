namespace System.Threading
{
	public abstract class PipeObject : Node<PipeObject>
	{
		#region Method
		public abstract void Receive(object Data);
		#endregion
	}
}