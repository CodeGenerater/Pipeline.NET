using System.Linq;
using System.Collections.Generic;

namespace System.Threading
{
	public class Pipeline : PipeObject
	{
		#region Constructor
		public Pipeline()
		{
			PipeCollection = new NodeList<PipeObject>();
		}

		public Pipeline(IEnumerable<PipeObject> PipeCollection) : this()
		{
			foreach (var each in PipeCollection)
				this.PipeCollection.Add(each);
		}
		#endregion

		#region Property
		public override PipeObject Next
		{
			set
			{
				base.Next = value;
				PipeCollection.Last().Next = value;
			}
			get
			{
				return base.Next;
			}
		}

		public NodeList<PipeObject> PipeCollection
		{
			private set;
			get;
		}
		#endregion

		#region Method
		public override void Receive(object Data)
		{
			PipeCollection.First()?.Receive(Data);
		}
		#endregion
	}
}