using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Test_Pipeline
{
	class Program
	{
		static void Main(string[] args)
		{
			Initialize();

			Pipeline PL = new Pipeline();

			for (int i = 0; i < 3; i++)
				PL.Add(new TestPipe());

			for (int j = 0; j < 3; j++)
				PL.Receive(null);

			Thread.Sleep(3000);

			Finalize();
		}

		#region Method
		static void Initialize()
		{
			T.Start();
		}

		static void Finalize()
		{
			T.Abort();
		}
		#endregion

		#region Threading
		static Thread T = new Thread(ThreadProc);

		public static ConcurrentQueue<string> MessageQueue = new ConcurrentQueue<string>();

		static void ThreadProc()
		{
			while (true)
			{
				string Message = null;
				if (MessageQueue.TryDequeue(out Message))
					Console.WriteLine(Message);
				else
					Thread.Sleep(10);
			}
		}
		#endregion

		#region Pipe
		class TestPipe : SingleCapacityPipe
		{
			public TestPipe()
			{
				Serial = _Serial++;
			}

			static int _Serial;

			int Serial;

			int Count;

			protected override object Process(object Data)
			{
				Count++;
				Console.WriteLine(string.Format("[{0,3}][{1}]", Serial, Count));
				//MessageQueue.Enqueue(string.Format("[{0,3}][{1}]", Serial, Count));
				return null;
			}
		}
		#endregion
	}
}