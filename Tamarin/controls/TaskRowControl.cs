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
	/// <summary>
	/// One row in the TaskTableControl.
	/// Displays one Task.
	/// </summary>
	public class TaskRowControl : TamarinRowControl
	{
		/// <summary>
		/// Triggered when size or location of this row changed.
		/// Should only be passed to next row in the table.
		/// </summary>
		public event EventHandler RowLocationChanged;
		/// <summary>Some control in row got focus.</summary>
		public event EventHandler RowGotFocus;
		/// <summary>Data edited.</summary>
		public event EventHandler RowIndexChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler RowTitleChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler RowStatusChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler RowCategoryChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler RowDeleted;

		private TitleTextBox titleBox;
		private StatusComboBox statusComboBox;
		private CategoryComboBox categoryComboBox;

		public TaskRowControl(int rowIndex, Task task, List<string> statusOptions, List<string> categoryOptions) : base(rowIndex)
		{
			InitControls(task, statusOptions, categoryOptions);
			//TODO set tab indexes
		}

		protected void InitControls(Task task, List<string> statusOptions, List<string> categoryOptions)
		{
			Button addButton = NewButton("+", Add_Click);
			addButton.Location = new Point(0, 0);
			addButton.AutoSize = true;
			this.Controls.Add(addButton);

			TextBox rowNumberBox = NewTextBox("RowNumberTextBox", rowIndex.ToString());
			rowNumberBox.GotFocus += new EventHandler(Row_GotFocus);
			rowNumberBox.LostFocus += new EventHandler(RowIndex_LostFocus);
			rowNumberBox.Margin = new Padding(0);
			rowNumberBox.TabStop = false;
			//rowNumberBox.KeyDown += new KeyEventHandler(rowNumberTextBox_KeyDown);
			//rowNumberBox.KeyUp += new KeyEventHandler(rowNumberTextBox_KeyUp);
			this.Controls.Add(rowNumberBox);

			Label idLabel = NewDataLabel("Id", task.Id.ToString());
			this.Controls.Add(idLabel);

			titleBox = new TitleTextBox("TitleTextBox", task.Description);
			titleBox.GotFocus += new EventHandler(Row_GotFocus);
			titleBox.TextChanged += new EventHandler(Row_TitleChanged);
			//titleBox.KeyDown += new KeyEventHandler(titleTextBox_KeyDown);
			//titleBox.KeyUp += new KeyEventHandler(titleTextBox_KeyUp);
			titleBox.TabIndex = 1;
			this.Controls.Add(titleBox);

			statusComboBox = new StatusComboBox(statusOptions, task.Status);
			statusComboBox.SelectedIndexChanged += new EventHandler(Row_StatusChanged);
			this.Controls.Add(statusComboBox);

			categoryComboBox = new CategoryComboBox(categoryOptions, task.Category);
			categoryComboBox.SelectedIndexChanged += new EventHandler(Row_CategoryChanged);
			this.Controls.Add(categoryComboBox);

			Label createLabel = NewDataLabel("CreateDate", task.CreateDateString);
			createLabel.Margin = new Padding(0);
			createLabel.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add(createLabel);

			Label doneLabel = NewDataLabel("DoneDate", task.DoneDateString);
			doneLabel.Margin = new Padding(0);
			doneLabel.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add(doneLabel);

			Button deleteButton = NewButton("X", Row_Deleted);
			this.Controls.Add(deleteButton);

			ArrangeControlsLeftToRight();
		}

		public override void SetRowIndex(int rowIndex)
		{
			base.SetRowIndex(rowIndex);
			//TODO Update tab indexes
		}

		#region Event Handlers

		public void Row_GotFocus(object sender, EventArgs e)
		{
			if(RowGotFocus == null) return;
			RowGotFocus.Invoke(this, new EventArgs());
		}

		public void RowIndex_LostFocus(object sender, EventArgs e)
		{
			if(RowIndexChanged == null) return;
			RowIndexChanged.Invoke(this, new EventArgs());
		}

		public void Row_TitleChanged(object sender, EventArgs e)
		{
			if(RowTitleChanged == null) return;
			RowTitleChanged.Invoke(this, new EventArgs());
		}

		public void Row_StatusChanged(object sender, EventArgs e)
		{
			if(RowStatusChanged == null) return;
			RowStatusChanged.Invoke(this, new EventArgs());
		}

		public void Row_CategoryChanged(object sender, EventArgs e)
		{
			if(RowCategoryChanged == null) return;
			RowCategoryChanged.Invoke(this, new EventArgs());
		}

		public void Row_Deleted(object sender, EventArgs e)
		{
			if(RowDeleted == null) return;
			RowDeleted.Invoke(this, new EventArgs());
		}

		#endregion

		public void FocusOnTitle()
		{
			titleBox.Focus();
		}

		protected void OnPreviousRowLocationChanged(object sender, EventArgs e)
		{
			//TODO
		}

		public void OnStatusesChanged(object sender, ListEventArgs e)
		{
			statusComboBox.UpdateOptions(e.Values.ToList());
		}

		public void OnCategoriesChanged(object sender, ListEventArgs e)
		{
			categoryComboBox.UpdateOptions(e.Values.ToList());
		}

		private Label NewDataLabel(string name, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = Settings.REGULAR_FONT;
			label.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			label.Size = new System.Drawing.Size(21, 24);
			label.Name = name;
			label.Text = text;
			return label;
		}

		private TextBox NewTextBox(string name, string text = null)
		{
			TextBox textBox = new TextBox();
			textBox.Dock = System.Windows.Forms.DockStyle.Top;
			textBox.Font = Settings.REGULAR_FONT;
			textBox.Name = name;
			textBox.Text = text;
			textBox.Size = new System.Drawing.Size(119, 22);
			return textBox;
		}

	}
}
