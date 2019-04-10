using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	public class DataLabel : Label //AutoLabel
	{
		public DataLabel(string name, string text)
		{
			this.Font = Settings.REGULAR_LABEL_FONT;
			this.Padding = new Padding(0);
			this.TextAlign = ContentAlignment.MiddleLeft;
			this.AutoSize = false;
			this.Text = text;
			this.Name = name;
		}
	}
}
