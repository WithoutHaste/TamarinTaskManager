using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManagerX
{
	public partial class StatusForm : Form
	{
		public StatusForm(string[] activeStatuses, string[] inactiveStatuses)
		{
			InitializeComponent();

			activeTextBox.Text = String.Join("\n", activeStatuses);
			inactiveTextBox.Text = String.Join("\n", inactiveStatuses);
		}

		public string[] GetActiveStatuses()
		{
			return activeTextBox.Text.Split('\n');
		}

		public string[] GetInactiveStatuses()
		{
			return inactiveTextBox.Text.Split('\n');
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}
	}
}
