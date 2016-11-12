using System;

namespace System.Threading
{
	public interface IInitializable : IDisposable
	{
		void Initialize();
	}
}