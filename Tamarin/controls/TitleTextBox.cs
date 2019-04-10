using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	/// <summary>
	/// The text box that contains the title/full text of the task.
	/// </summary>
	public class TitleTextBox : RichTextBox
	{
		private static Font FONT = Settings.REGULAR_FONT;
		private static int LINE_HEIGHT = Settings.REGULAR_CHAR_HEIGHT;

		public TitleTextBox(string controlName, string text)
		{
			this.Font = FONT;
			this.Name = controlName;
			this.Text = text;
			this.Margin = new Padding(0);
			this.BorderStyle = BorderStyle.None; //on RichTextBox, FixedSingle displays as Fixed3D

			this.MouseWheel += new MouseEventHandler(Utilities.PassMouseWheelToParent);
			this.SizeChanged += new EventHandler(OnSizeChanged);
			this.TextChanged += new EventHandler(OnTextChanged);
		}

		public bool CursorOnFirstLine()
		{
			int cursorIndex = this.SelectionStart;
			int lineCount = CountLines();
			return (lineCount == 1 || this.GetFirstCharIndexFromLine(1) > cursorIndex);
		}

		public bool CursorOnLastLine()
		{
			int cursorIndex = this.SelectionStart;
			int lineCount = CountLines();
			return (this.GetFirstCharIndexFromLine(lineCount - 1) <= cursorIndex);
		}

		private void SetHeightByText()
		{
			if(this.Width < 10)
				return; //wait until textBox is a likely real size before arranging it, otherwise the table layout gets artificially tall
			int newHeight = (LINE_HEIGHT * this.CountLines());
			if(newHeight == this.Size.Height)
				return; //do not go into a resizing loop
			this.Size = new Size(this.Width, newHeight);
		}

		private int CountLines()
		{
			int lineCount = 1;
			while(this.GetFirstCharIndexFromLine(lineCount) > -1)
			{
				lineCount++;
			}
			return lineCount;
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			this.SetHeightByText();
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			this.SetHeightByText();
		}
	}
}
