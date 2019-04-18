using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tamarin
{
	public class ListEventArgs : EventArgs
	{
		public string[] Values { get; private set; }

		public ListEventArgs(string[] values)
		{
			Values = values;
		}
	}
}
