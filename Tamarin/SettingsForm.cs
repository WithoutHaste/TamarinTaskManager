using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();

			showTaskIdsCheckBox.Checked = Properties.Settings.Default.ShowTaskIds;
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.ShowTaskIds = showTaskIdsCheckBox.Checked;
			Properties.Settings.Default.Save();

			this.DialogResult = DialogResult.OK;
		}
	}
}
