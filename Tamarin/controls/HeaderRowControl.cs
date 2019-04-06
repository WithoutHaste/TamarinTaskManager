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
			InitControls();
		}

		private void InitControls()
		{
			Button addButton = NewButton("+", Add_Click);
			addButton.Location = new Point(0, 0);
			addButton.AutoSize = true;
			this.Controls.Add(addButton);

			this.Controls.Add(new TitleLabel("Row"));
			this.Controls.Add(new TitleLabel("Id"));
			this.Controls.Add(new TitleLabel("Description"));
			this.Controls.Add(new TitleLabel("Status"));
			this.Controls.Add(new TitleLabel("Category"));
			this.Controls.Add(new TitleLabel("Created"));
			this.Controls.Add(new TitleLabel("Finished"));

			ArrangeControlsLeftToRight();
		}
	}
}
