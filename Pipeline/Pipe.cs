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
		Thread T;
		AutoResetEvent ResetEvent;
		AutoResetEvent PrevResetEvent;
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
				PrevResetEvent.Reset();

			PrevResetEvent.WaitOne();
			Save(Data);
			ResetEvent.Set();
		}

		protected object Request()
		{
			ResetEvent.WaitOne();
			object Data = Load();
			PrevResetEvent.Set();
			return Data;
		}

		protected abstract object Process(object Data);

		protected abstract void Save(object Data);

		protected abstract object Load();

		public virtual void Initialize()
		{
			CanReceive = true;
			ResetEvent = new AutoResetEvent(false);
			PrevResetEvent = new AutoResetEvent(true);

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
			if (ResetEvent != null)
			{
				ResetEvent.Dispose();
				ResetEvent = null;
			}
			if (PrevResetEvent != null)
			{
				PrevResetEvent.Dispose();
				PrevResetEvent = null;
			}
		}
		#endregion

		#region Helper
		void ThreadProc()
		{
			while (true)
			{
				object Data = Process(Request());

				Next?.Receive(Data);
			}
		}
		#endregion
	}
}