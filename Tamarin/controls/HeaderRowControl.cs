using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	public class HeaderRowControl : TamarinRowControl
	{
		public HeaderRowControl() : base(0)
		{
			this.Height = (int)(Settings.TITLE_CHAR_HEIGHT * 2);
			InitControls();
		}

		private void InitControls()
		{
			this.Controls.Add(NewButton("+", Add_Click));
			this.Controls.Add(new TitleLabel("Row"));
			this.Controls.Add(new TitleLabel("Id"));
			this.Controls.Add(new TitleLabel("Description"));
			this.Controls.Add(new TitleLabel("Status"));
			this.Controls.Add(new TitleLabel("Category"));
			this.Controls.Add(new TitleLabel("Created"));
			this.Controls.Add(new TitleLabel("Finished"));
			this.Controls.Add(new TitleLabel("  "));
			SetupColumns(this.Controls);
		}
	}
}
