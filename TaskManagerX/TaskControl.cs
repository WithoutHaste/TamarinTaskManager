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
	public partial class TaskControl : UserControl
	{
		public int RowNumber {
			get {
				return Int32.Parse(this.rowLabel.Text);
			}
			set {
				this.rowLabel.Text = value.ToString();
			}
		}

		public TaskControl()
		{
			InitializeComponent();
		}

		public TaskControl(string[] statuses, string[] categories)
		{
			InitializeComponent();

			this.statusComboBox.DataSource = statuses;
			this.categoryComboBox.DataSource = categories;
		}

		private void TaskControl_Load(object sender, EventArgs e)
		{
			this.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		}

		public void ShowOnlyAddButton()
		{
			this.rowLabel.Visible = false;
			this.idLabel.Visible = false;
			this.titleTextBox.Visible = false;
			this.statusComboBox.Visible = false;
			this.categoryComboBox.Visible = false;
			this.createDateLabel.Visible = false;
			this.statusDateLabel.Visible = false;
		}

		public void SetAddEventHandler(EventHandler handler)
		{
			this.addButton.Click += handler;
		}
	}
}
