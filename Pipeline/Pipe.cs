namespace System.Threading
{
	public abstract class Pipe : IPipe
	{
		#region Constructor
		public Pipe()
		{
			Initialize();
		}
		#endregion

		#region Finalizer
		~Pipe()
		{
			Dispose();
		}
		#endregion

		#region Field
		AutoResetEvent EventForPrev;
		AutoResetEvent EventForCurrent;
		Thread T;
		#endregion

		#region Property
		protected bool CanReceive
		{
			set;
			get;
		}

		public IPipe Prev
		{
			get;
			set;
		}

		public IPipe Next
		{
			get;
			set;
		}
		#endregion

		#region Method
		public void Receive(object Data)
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

		public virtual void Initialize()
		{
			CanReceive = true;

			EventForPrev = new AutoResetEvent(true);
			EventForCurrent = new AutoResetEvent(false);

			T = new Thread(ThreadProc);
			T.Start();
		}

		public virtual void Dispose()
		{
			if (T != null)
			{
				T.Abort();
				T = null;
			}
			if (EventForPrev != null)
			{
				EventForPrev.Dispose();
				EventForPrev = null;
			}
			if (EventForCurrent != null)
			{
				EventForCurrent.Dispose();
				EventForCurrent = null;
			}
		}
		#endregion

		#region Helper
		void ThreadProc()
		{
			while (true)
			{
				Next?.Receive(Process(Request()));
			}
		}
		#endregion
	}
}