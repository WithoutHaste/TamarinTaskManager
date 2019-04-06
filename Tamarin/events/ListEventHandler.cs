using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tamarin
{
	public delegate void ListEventHandler(object sender, ListEventArgs e);

	public class ListEventArgs : EventArgs
	{
		public string[] Values { get; private set; }

		public ListEventArgs(string[] values)
		{
			Values = values;
		}
	}
}
