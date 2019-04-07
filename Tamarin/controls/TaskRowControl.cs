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
		public event IntEventHandler RowIndexChanged;
		/// <summary>Data edited.</summary>
		public event StringEventHandler RowTitleChanged;
		/// <summary>Move focus to another row.</summary>
		public event GoToRowEventHandler GoToRow;
		/// <summary>Data edited.</summary>
		public event StringEventHandler RowStatusChanged;
		/// <summary>Data edited.</summary>
		public event StringEventHandler RowCategoryChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler RowDeleted;

		private TextBox rowNumberBox;
		private TitleTextBox titleBox;
		private StatusComboBox statusComboBox;
		private CategoryComboBox categoryComboBox;

		public TaskRowControl(int rowIndex, Task task, List<string> statusOptions, List<string> categoryOptions) : base(rowIndex)
		{
			InitControls(task, statusOptions, categoryOptions);
		}

		protected void InitControls(Task task, List<string> statusOptions, List<string> categoryOptions)
		{
			Button addButton = NewButton("+", Add_Click);
			addButton.Location = new Point(0, 0);
			addButton.AutoSize = true;
			this.Controls.Add(addButton);

			rowNumberBox = NewTextBox("RowNumberTextBox", rowIndex.ToString());
			rowNumberBox.GotFocus += new EventHandler(Row_GotFocus);
			rowNumberBox.LostFocus += new EventHandler(RowIndex_LostFocus);
			rowNumberBox.Margin = new Padding(0);
			rowNumberBox.TabStop = false;
			rowNumberBox.KeyDown += new KeyEventHandler(RowIndex_KeyDown);
			rowNumberBox.KeyUp += new KeyEventHandler(RowIndex_KeyUp);
			this.Controls.Add(rowNumberBox);

			Label idLabel = NewDataLabel("Id", task.Id.ToString());
			idLabel.AutoSize = true;
			this.Controls.Add(idLabel);

			titleBox = new TitleTextBox("TitleTextBox", task.Description);
			titleBox.GotFocus += new EventHandler(Row_GotFocus);
			titleBox.TextChanged += new EventHandler(Row_TitleChanged);
			titleBox.KeyDown += new KeyEventHandler(Title_KeyDown);
			titleBox.KeyUp += new KeyEventHandler(Title_KeyUp);
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
			SetTabIndexes();
		}

		public override void SetRowIndex(int rowIndex)
		{
			base.SetRowIndex(rowIndex);
			SetTabIndexes();
		}

		private void SetTabIndexes()
		{
			titleBox.TabIndex = (rowIndex * 10) + 1;
			statusComboBox.TabIndex = (rowIndex * 10) + 2;
			categoryComboBox.TabIndex = (rowIndex * 10) + 3;
		}

		public void SetTitle(string text)
		{
			titleBox.Text = text;
		}

		public void SetStatus(string status)
		{
			if(!statusComboBox.Items.Contains(status))
				statusComboBox.Items.Add(status);
			statusComboBox.SelectedIndex = statusComboBox.Items.IndexOf(status);
		}

		public void SetCategory(string category)
		{
			if(!categoryComboBox.Items.Contains(category))
				categoryComboBox.Items.Add(category);
			categoryComboBox.SelectedIndex = categoryComboBox.Items.IndexOf(category);
		}

		#region Event Handlers

		public void Row_GotFocus(object sender, EventArgs e)
		{
			if(RowGotFocus == null) return;
			RowGotFocus.Invoke(this, new EventArgs());
		}

		public void RowIndex_LostFocus(object sender, EventArgs e)
		{
			int newRow;
			if(Int32.TryParse(rowNumberBox.Text, out newRow))
			{
				if(newRow != rowIndex)
				{
					//if row number changed, move the row
					if(RowIndexChanged == null) return;
					RowIndexChanged.Invoke(this, new IntEventArgs(newRow));
					return;
				}
			}
			//if row number is not a valid value, change it back to its previous value
			rowNumberBox.Text = rowIndex.ToString();
		}

		private void RowIndex_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; //stop the error-ding from sounding
			}
		}

		private void RowIndex_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				FocusOnTitle();
				e.Handled = true;
			}
		}

		private bool IsDownArrow(KeyEventArgs e)
		{
			return (e.KeyCode == Keys.Down);
		}
		private bool IsUpArrow(KeyEventArgs e)
		{
			return (e.KeyCode == Keys.Up);
		}
		private bool IsControlKey(KeyEventArgs e)
		{
			return e.Control;
		}
		private bool IsControlDownArrow(KeyEventArgs e)
		{
			return (IsDownArrow(e) && IsControlKey(e));
		}
		private bool IsControlUpArrow(KeyEventArgs e)
		{
			return (IsUpArrow(e) && IsControlKey(e));
		}
		private void GoTo(int rowIndex, bool lastLine)
		{
			if(GoToRow == null) return;
			GoToRow.Invoke(this, new GoToRowEventArgs(rowIndex, lastLine));
		}
		public void Title_KeyDown(object sender, KeyEventArgs e)
		{
			if(IsControlDownArrow(e) ||
				(IsDownArrow(e) && titleBox.CursorOnLastLine()))
			{
				//go to next row
				GoTo(rowIndex + 1, lastLine: false);
				e.Handled = true;
				return;
			}
			if(IsControlUpArrow(e) ||
				(IsUpArrow(e) && titleBox.CursorOnFirstLine()))
			{
				//go to previous row
				GoTo(rowIndex - 1, lastLine: true);
				e.Handled = true;
				return;
			}
		}

		private void Title_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Control && e.KeyCode == Keys.N)
			{
				Add_Click(sender, new EventArgs());
			}
		}

		public void Row_TitleChanged(object sender, EventArgs e)
		{
			if(RowTitleChanged == null) return;
			RowTitleChanged.Invoke(this, new StringEventArgs(titleBox.Text));
		}

		public void Row_StatusChanged(object sender, EventArgs e)
		{
			if(RowStatusChanged == null) return;
			RowStatusChanged.Invoke(this, new StringEventArgs(statusComboBox.Text));
		}

		public void Row_CategoryChanged(object sender, EventArgs e)
		{
			if(RowCategoryChanged == null) return;
			RowCategoryChanged.Invoke(this, new StringEventArgs(categoryComboBox.Text));
		}

		public void Row_Deleted(object sender, EventArgs e)
		{
			if(RowDeleted == null) return;
			RowDeleted.Invoke(this, new EventArgs());
		}

		#endregion

		public void FocusOnTitle(bool lastLine)
		{
			if(!lastLine)
				FocusOnTitle(0, 0); //beginning of first line
			else
				FocusOnTitle(titleBox.Text.Length, 0); //beginning of last line
		}

		public void FocusOnTitle(int caret = -1, int selectionLength = 0)
		{
			titleBox.Focus();
			if(caret == -1)
			{
				caret = 0;
				selectionLength = 0;
			}
			titleBox.Select(caret, selectionLength);
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
