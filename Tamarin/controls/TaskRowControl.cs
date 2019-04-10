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
		/// Should only be acted on by next row in the table.
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

		private readonly int minHeight;

		public TaskRowControl(int rowIndex, Task task, List<string> statusOptions, List<string> categoryOptions) : base(rowIndex)
		{
			InitControls(task, statusOptions, categoryOptions);
			minHeight = this.Height;
		}

		protected void InitControls(Task task, List<string> statusOptions, List<string> categoryOptions)
		{
			this.Controls.Add(NewButton(Settings.SYMBOL_ADD, Add_Click));

			rowNumberBox = NewTextBox("RowNumberTextBox", rowIndex.ToString());
			rowNumberBox.TextAlign = HorizontalAlignment.Center;
			rowNumberBox.GotFocus += new EventHandler(Row_GotFocus);
			rowNumberBox.LostFocus += new EventHandler(RowIndex_LostFocus);
			rowNumberBox.KeyDown += new KeyEventHandler(RowIndex_KeyDown);
			rowNumberBox.KeyUp += new KeyEventHandler(RowIndex_KeyUp);
			rowNumberBox.TabStop = false;
			this.Controls.Add(rowNumberBox);

			Label idLabel = new DataLabel("Id", task.Id.ToString());
			idLabel.TextAlign = ContentAlignment.TopCenter;
			this.Controls.Add(idLabel);

			titleBox = new TitleTextBox("TitleTextBox", task.Description);
			titleBox.GotFocus += new EventHandler(Row_GotFocus);
			titleBox.TextChanged += new EventHandler(Row_TitleChanged);
			titleBox.KeyDown += new KeyEventHandler(Title_KeyDown);
			titleBox.KeyUp += new KeyEventHandler(Title_KeyUp);
			titleBox.SizeChanged += new EventHandler(Title_SizeChanged);
			this.Controls.Add(titleBox);

			statusComboBox = new StatusComboBox(statusOptions, task.Status);
			statusComboBox.SelectedIndexChanged += new EventHandler(Row_StatusChanged);
			this.Controls.Add(statusComboBox);

			categoryComboBox = new CategoryComboBox(categoryOptions, task.Category);
			categoryComboBox.SelectedIndexChanged += new EventHandler(Row_CategoryChanged);
			this.Controls.Add(categoryComboBox);

			Label createLabel = new DataLabel("CreateDate", task.CreateDateString);
			createLabel.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add(createLabel);

			Label doneLabel = new DataLabel("DoneDate", task.DoneDateString);
			doneLabel.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add(doneLabel);

			this.Controls.Add(NewButton(Settings.SYMBOL_MULTIPLY, Row_Deleted));

			SetupColumns(this.Controls);
			SetTabIndexes();
		}

		public override void SetRowIndex(int rowIndex)
		{
			base.SetRowIndex(rowIndex);
			rowNumberBox.Text = rowIndex.ToString();
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

		#region Inner Event Handlers

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

		public void Title_KeyDown(object sender, KeyEventArgs e)
		{
			if(IsControlDownArrow(e) ||
				(IsDownArrow(e) && titleBox.IsCaretOnLastLine()))
			{
				//go to next row
				GoTo(rowIndex + 1, titleBox.CaretX, lastLine: false);
				e.Handled = true;
				return;
			}
			if(IsControlUpArrow(e) ||
				(IsUpArrow(e) && titleBox.IsCaretOnFirstLine()))
			{
				//go to previous row
				GoTo(rowIndex - 1, titleBox.CaretX, lastLine: true);
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

		private void Title_SizeChanged(object sender, EventArgs e)
		{
			TitleTextBox box = (sender as TitleTextBox);
			int newHeight = Math.Max(minHeight, box.Height);
			if(this.Height != newHeight) //don't go into resizing loop
			{
				this.Height = newHeight;
			}
			InvokeRowLocationChanged();
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

		#endregion

		#region Outside Event Handlers

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

		#endregion

		#region Trigger Events

		private void GoTo(int rowIndex, int caretX, bool lastLine)
		{
			if(GoToRow == null) return;
			GoToRow.Invoke(this, new GoToRowEventArgs(rowIndex, caretX, lastLine));
		}

		private void InvokeRowLocationChanged()
		{
			if(RowLocationChanged == null) return;
			RowLocationChanged.Invoke(this, EventArgs.Empty);
		}

		#endregion

		public void FocusOnTitle(int caretX, bool lastLine)
		{
			if(!lastLine) //go to position in first line
			{
				int caretCharIndex = titleBox.ConvertXToCaretOnFirstLine(caretX);
				FocusOnTitle(caretCharIndex, 0);
			}
			else //go to position in last line
			{
				int caretCharIndex = titleBox.ConvertXToCaretOnLastLine(caretX);
				FocusOnTitle(caretCharIndex, 0);
			}
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

		private TextBox NewTextBox(string name, string text = null)
		{
			TextBox textBox = new TextBox();
			textBox.Font = Settings.REGULAR_FONT;
			textBox.Name = name;
			textBox.Text = text;
			return textBox;
		}

	}
}
