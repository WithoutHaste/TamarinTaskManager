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
		public HeaderRowControl(bool showColumnsForActive) : base(0, showColumnsForActive)
		{
			InitControls();
		}

		private void InitControls()
		{
			this.Controls.Add(NewButton("+", OnAddButtonClick));
			this.Controls.Add(new TitleLabel("Row"));

			Label idLabel = new TitleLabel("Id");
			idLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.Controls.Add(idLabel);

			this.Controls.Add(new TitleLabel("Description"));
			this.Controls.Add(new TitleLabel("Status"));
			this.Controls.Add(new TitleLabel("Category"));

			Label createdLabel = new TitleLabel("Created");
			createdLabel.TextAlign = ContentAlignment.MiddleRight;
			this.Controls.Add(createdLabel);

			Label finishedLabel = new TitleLabel("Finished");
			finishedLabel.TextAlign = ContentAlignment.MiddleRight;
			this.Controls.Add(finishedLabel);
	
			this.Controls.Add(new TitleLabel("  "));
			SetupColumns(this.Controls);
		}
	}
}
