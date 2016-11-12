using System;
using System.Threading;

namespace Test_Pipeline
{
	class Program
	{
		#region Main
		static void Main(string[] args)
		{
			Pipeline PL = new Pipeline();

			for (int i = 0; i < 3; i++)
				PL.Add(new TestPipe());

			for (int j = 0; j < 5; j++)
			{
				PL.Receive(j);
				RandomEffect(PL);
			}

			Console.ReadLine();

			PL.Dispose();
		}
		#endregion

		#region Field
		static Random R = new Random();
		#endregion

		#region Method
		static void RandomEffect(Pipeline Pipeline)
		{
			switch (R.Next(3))
			{
				case 0:
					Pipeline.Add(new TestPipe());
					Console.WriteLine("Add");
					break;
				case 1:
					if (Pipeline.Count > 3)
					{
						Pipeline.RemoveAt(R.Next(Pipeline.Count));
						Console.WriteLine("RemoveAt");
					}
					else
						goto case 0;
					break;
				case 2:
					Pipeline.Insert(R.Next(Pipeline.Count), new TestPipe());
					Console.WriteLine("Insert");
					break;
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

			protected override object Process(object Data)
			{
				Console.WriteLine(string.Format("[{0,3}][{1}]", Serial, Data));
				return Data;
			}
		}
		#endregion
	}
}