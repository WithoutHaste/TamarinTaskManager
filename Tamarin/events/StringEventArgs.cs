using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tamarin
{
	public class StringEventArgs : EventArgs
	{
		public string Value { get; private set; }

		public StringEventArgs(string value)
		{
			Value = value;
		}
	}
}
