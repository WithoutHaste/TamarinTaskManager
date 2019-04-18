using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tamarin
{
	public class ShowColumnEventArgs : EventArgs
	{
		public bool Show { get; private set; }
		public int ColumnIndex { get; private set; }

		public ShowColumnEventArgs(bool show, int columnIndex)
		{
			Show = show;
			ColumnIndex = columnIndex;
		}
	}
}
