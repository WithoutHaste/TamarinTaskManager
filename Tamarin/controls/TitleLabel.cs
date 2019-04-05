using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	/// <summary>
	/// A header.
	/// </summary>
	public class TitleLabel : Label
	{
		public TitleLabel(string text)
		{
			this.AutoSize = true;
			this.Font = Settings.TITLE_FONT;
			this.Padding = new System.Windows.Forms.Padding(left: 0, top: 8, right: 0, bottom: 0);
			this.Text = text;
		}
	}
}
