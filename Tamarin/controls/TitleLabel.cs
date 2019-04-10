using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	/// <summary>
	/// A row or column header.
	/// </summary>
	public class TitleLabel : Label
	{
		public TitleLabel(string text)
		{
			this.Font = Settings.TITLE_FONT;
			this.Padding = new Padding(0);
			this.TextAlign = ContentAlignment.MiddleLeft;
			this.AutoSize = false;
			this.Text = text;
		}
	}
}
