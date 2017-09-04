using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManagerX
{
	public partial class TaskTableToolStrip : UserControl
	{
		private TaskTableControl taskTableControl;

		public TaskTableToolStrip(TaskTableControl taskTableControl)
		{
			this.taskTableControl = taskTableControl;

			InitializeComponent();

			this.Dock = DockStyle.Bottom;
		}

		private void activeRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if(!(sender as RadioButton).Checked)
				return;
			taskTableControl.ShowTaskSheet(active: true);
		}

		private void inactiveRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if(!(sender as RadioButton).Checked)
				return;
			taskTableControl.ShowTaskSheet(active: false);
		}
	}
}
