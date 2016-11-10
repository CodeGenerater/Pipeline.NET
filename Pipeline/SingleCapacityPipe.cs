namespace System.Threading
{
	public abstract class SingleCapacityPipe : PipeBase
	{
		#region Field
		object _Data;
		#endregion

		#region Method
		protected override object Load()
		{
			return _Data;
		}

		protected override void Save(object Data)
		{
			_Data = Data;
		}
		#endregion
	}
}