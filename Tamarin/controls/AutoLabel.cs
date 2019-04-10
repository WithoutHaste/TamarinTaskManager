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
	/// Automatically sets font size to best-fit the label size.
	/// </summary>
	public class AutoLabel : Label
	{
		public int MaxFontSize = 20;
		public readonly int MinFontSize = 5;

		public AutoLabel()
		{
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			SetFontSize();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			SetFontSize();
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			SetFontSize();
		}

		private void SetFontSize()
		{
			if(String.IsNullOrEmpty(this.Text))
				return;
			if(this.Font == null)
				return;
			if(this.Width < 5 || this.Height < 5)
				return;

			using(Graphics graphics = this.CreateGraphics())
			{
				Font maxWorkingFont = null;
				for(int f = MinFontSize; f <= MaxFontSize; f++)
				{
					using(Font font = new Font(this.Font.FontFamily, f, this.Font.Style))
					{
						//SizeF size = graphics.MeasureString(this.Text, font); //using this resulted in far-too-large text
						Size size = TextRenderer.MeasureText(this.Text, font); //using this results in correctly sized text
						if(size.Width > this.ClientRectangle.Width)
							break;
						if(size.Height > this.ClientRectangle.Height)
							break;
						maxWorkingFont = font;
					}
				}
				if(maxWorkingFont != null)
				{
					this.Font = maxWorkingFont;
				}
			}
		}
	}
}
