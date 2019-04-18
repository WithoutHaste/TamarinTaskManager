using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tamarin
{
	public class IntEventArgs : EventArgs
	{
		public int Value { get; private set; }

		public IntEventArgs(int value)
		{
			Value = value;
		}
	}
}
