namespace System.Threading
{
	public abstract class SingleCapacityPipe : Pipe
	{
		#region Field
		object _Data;
		#endregion

		#region Method
		protected override object Load()
		{
			CanReceive = true;
			return _Data;
		}

		protected override void Save(object Data)
		{
			CanReceive = false;
			_Data = Data;
		}
		#endregion
	}
}