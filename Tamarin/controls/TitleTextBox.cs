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

		#region Properties

		/// <summary>
		/// Number of characters from the beginning of the text box to the caret.
		/// </summary>
		public int CaretCharIndex {
			get {
				return this.SelectionStart;
			}
		}

		/// <summary>
		/// Number of pixels from the left side of the text box to the caret.
		/// </summary>
		public int CaretX {
			get {
				return this.GetPositionFromCharIndex(this.SelectionStart).X;
			}
		}

		#endregion

		public TitleTextBox(string controlName, string text)
		{
			this.Font = FONT;
			this.Name = controlName;
			this.Text = text;
			this.Margin = new Padding(0);
			this.BorderStyle = BorderStyle.None; //on RichTextBox, FixedSingle displays as Fixed3D
			//this.ScrollBars = RichTextBoxScrollBars.None; //causes error where top line is always scrolled out of view

			this.MouseWheel += new MouseEventHandler(Utilities.PassMouseWheelToParent);
			this.SizeChanged += new EventHandler(OnSizeChanged);
			this.TextChanged += new EventHandler(OnTextChanged);

			//since font is displaying at unreliable sizes, just trust the center sizing measurement
			//CalcRenderedLineHeight();
		}

		public bool IsCaretOnFirstLine()
		{
			int cursorIndex = this.SelectionStart;
			int lineCount = CountLines();
			return (lineCount == 1 || this.GetFirstCharIndexFromLine(1) > cursorIndex);
		}

		public bool IsCaretOnLastLine()
		{
			int cursorIndex = this.SelectionStart;
			int lineCount = CountLines();
			return (this.GetFirstCharIndexFromLine(lineCount - 1) <= cursorIndex);
		}

		/// <summary>
		/// Returns the caret position for a point on the first line that is <paramref name='x'/> distance from the Left of the TextBox.
		/// </summary>
		public int ConvertXToCaretOnFirstLine(int x)
		{
			return this.GetCharIndexFromPosition(new Point(x, LINE_HEIGHT / 2));
		}

		/// <summary>
		/// Returns the caret position for a point on the last line that is <paramref name='x'/> distance from the Left of the TextBox.
		/// </summary>
		public int ConvertXToCaretOnLastLine(int x)
		{
			return this.GetCharIndexFromPosition(new Point(x, this.Height - (LINE_HEIGHT / 2)));
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
/*
		#region Determine Rendered Line Height

		private void CalcRenderedLineHeight()
		{
			using(Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) //using this.CreateGraphics() here somehow interrupts the sizing of the control, causing a layout error
			{
				SizeF size = graphics.MeasureString("TEST", this.Font);
				LINE_HEIGHT = (int)Math.Ceiling(size.Height);
			}
		}

		#endregion
*/
	}
}
