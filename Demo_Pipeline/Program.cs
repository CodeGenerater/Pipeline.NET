using System;
using System.IO;
using System.Threading;

namespace Demo_Pipeline
{
	class Program
	{
		static void Main(string[] args)
		{
			DemoPipe P1 = new DemoPipe();
			DemoPipe P2 = new DemoPipe();
			DemoPipe P3 = new DemoPipe();

			P1.Next = P2; P2.Prev = P1;
			P2.Next = P3; P3.Prev = P2;

			for (int i = 0; i < 3; i++)
				P1.Receive(i + 1);

			Console.ReadLine();

			P1.Dispose();
			P2.Dispose();
			P3.Dispose();
		}
	}

	class DemoPipe : IDisposable
	{
		#region Constructor
		public DemoPipe()
		{
			CanReceive = true;
			T = new Thread(ThreadProc);
			T.Start();
			Serial = _Serial++;
		}
		#endregion

		#region Field
		Thread T;
		int Value;
		AutoResetEvent ResetEvent = new AutoResetEvent(false);
		AutoResetEvent PrevResetEvent = new AutoResetEvent(true);

		static int _Serial;
		#endregion

		#region Property
		public bool CanReceive
		{
			set;
			get;
		}

		public DemoPipe Prev
		{
			set;
			get;
		}

		public DemoPipe Next
		{
			set;
			get;
		}

		public int Serial
		{
			private set;
			get;
		}
		#endregion

		#region Method
		public void Receive(int Value)
		{
			Trace("Receive({0})", Value);
			Trace("CanReceive = {0}", CanReceive);

			if (!CanReceive)
				PrevResetEvent.Reset();

			Trace("Receive->Start Wait");
			PrevResetEvent.WaitOne();
			Trace("Receive->End Wait");

			Trace("Set Value {0} to {1}", this.Value, Value);
			this.Value = Value;
			CanReceive = false;
			Trace("Resume Current Thread");
			ResetEvent.Set();
		}

		public int Request()
		{
			Trace("Request->Start Wait");
			ResetEvent.WaitOne();
			Trace("Reqeust->End Wait");
			int Output = Value;
			CanReceive = true;
			Trace("Request return {0}", Output);
			PrevResetEvent.Set();
			return Output;
		}

		public void Dispose()
		{
			T.Abort();
			ResetEvent.Dispose();
			PrevResetEvent.Dispose();
		}
		#endregion

		#region Helper
		void ThreadProc()
		{
			while (true)
			{
				Trace("Start Request");
				int Value = Request();
				Trace("End Request");

				Value = Value * 10;

				Trace("Start Receive");
				Next?.Receive(Value);
				Trace("End Receive");
			}
		}

		void Trace(string Message)
		{
			Console.WriteLine("[{0}] : {1}", Serial, Message);
		}

		void Trace(string Format, params object[] Params)
		{
			Trace(string.Format(Format, Params));
		}
		#endregion
	}
}