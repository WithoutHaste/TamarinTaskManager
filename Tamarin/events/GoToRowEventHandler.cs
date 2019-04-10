using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tamarin
{
	public delegate void GoToRowEventHandler(object sender, GoToRowEventArgs e);

	public class GoToRowEventArgs : EventArgs
	{
		public int RowIndex { get; private set; }
		/// <duplicate cref='TitleTextBox.CaretX'/>
		public int CaretX { get; private set; }
		/// <summary>
		/// True: go to last line of text.
		/// False: go to first line of text.
		/// </summary>
		public bool LastLine { get; private set; }

		public GoToRowEventArgs(int rowIndex, int caretX, bool lastLine)
		{
			RowIndex = rowIndex;
			CaretX = caretX;
			LastLine = lastLine;
		}
	}
}
