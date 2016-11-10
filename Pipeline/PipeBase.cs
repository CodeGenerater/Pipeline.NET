namespace System.Threading
{
	public abstract class PipeBase : PipeObject
	{
		#region Field
		AutoResetEvent EventForPrev;
		AutoResetEvent EventForCurrent;
		#endregion

		#region Property
		public bool CanReceive
		{
			protected set;
			get;
		}
		#endregion

		#region Method
		public override void Receive(object Data)
		{
			if (!CanReceive)
				EventForPrev.Reset();

			EventForPrev.WaitOne();
			Save(Data);
			EventForCurrent.Set();
		}

		protected object Request()
		{
			EventForCurrent.WaitOne();
			object Data = Load();
			EventForPrev.Set();
			return Data;
		}

		protected abstract object Process(object Data);

		protected abstract void Save(object Data);

		protected abstract object Load();
		#endregion
	}
}