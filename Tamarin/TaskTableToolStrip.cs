using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	public partial class TaskTableToolStrip : UserControl
	{
		private TaskTableControl taskTableControl;

		public TaskTableToolStrip(TaskTableControl taskTableControl)
		{
			this.taskTableControl = taskTableControl;
			this.taskTableControl.ToolStrip = this;

			InitializeComponent();

			this.Dock = DockStyle.Bottom;

			SelectActiveInactive(active: true);
		}

		public void SelectActiveInactive(bool active)
		{
			if(active)
			{
				activeRadioButton.Checked = true;
				clearInactiveButton.Visible = false;
			}
			else
			{
				inactiveRadioButton.Checked = true;
				clearInactiveButton.Visible = true;
			}
		}

		private void activeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if(!(sender as RadioButton).Checked)
				return;
			taskTableControl.ShowTaskSheet(active: true);
			clearInactiveButton.Visible = false;
		}

		private void inactiveRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if(!(sender as RadioButton).Checked)
				return;
			taskTableControl.ShowTaskSheet(active: false);
			clearInactiveButton.Visible = true;
		}

		private void editStatusesButton_Click(object sender, EventArgs e)
		{
			taskTableControl.EditStatuses();
		}

		private void editCategoriesButton_Click(object sender, EventArgs e)
		{
			taskTableControl.EditCategories();
		}

		private void clearInactiveButton_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Permanently delete all inactive items?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if(result == DialogResult.Yes)
			{
				taskTableControl.ClearAllInactive();
			}
		}
	}
}
