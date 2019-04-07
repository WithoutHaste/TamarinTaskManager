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
			this.Font = Settings.TITLE_FONT;
			this.Padding = new Padding(0);
			this.Text = text;
		}
	}
}
