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
		public event EventHandler<IntEventArgs> IndexChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler<StringEventArgs> TitleChanged;
		/// <summary>Move focus to another row.</summary>
		public event EventHandler<GoToRowEventArgs> GoToRow;
		/// <summary>Data edited.</summary>
		public event EventHandler<StringEventArgs> StatusChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler<StringEventArgs> CategoryChanged;
		/// <summary>Data edited.</summary>
		public event EventHandler RowDeleted;

		private TextBox indexBox;
		private TitleTextBox titleBox;
		private StatusComboBox statusComboBox;
		private CategoryComboBox categoryComboBox;

		private readonly int minHeight;

		public int TaskId { get; private set; }
		public DateTime? DoneDate { get; private set; }

		public TaskRowControl(int rowIndex, Task task, List<string> statusOptions, List<string> categoryOptions) : base(rowIndex)
		{
			TaskId = task.Id;
			DoneDate = task.DoneDate;

			InitControls(task, statusOptions, categoryOptions);

			minHeight = this.Height;
		}

		protected void InitControls(Task task, List<string> statusOptions, List<string> categoryOptions)
		{
			this.Controls.Add(NewButton(Settings.SYMBOL_ADD, OnAddButtonClick));

			indexBox = NewTextBox("RowNumberTextBox", rowIndex.ToString());
			indexBox.TextAlign = HorizontalAlignment.Center;
			indexBox.GotFocus += new EventHandler(OnIndexGotFocus);
			indexBox.LostFocus += new EventHandler(OnIndexLostFocus);
			indexBox.KeyDown += new KeyEventHandler(OnIndexKeyDown);
			indexBox.KeyUp += new KeyEventHandler(OnIndexKeyUp);
			indexBox.TabStop = false;
			this.Controls.Add(indexBox);

			Label idLabel = new DataLabel("Id", task.Id.ToString());
			idLabel.TextAlign = ContentAlignment.TopCenter;
			this.Controls.Add(idLabel);

			titleBox = new TitleTextBox("TitleTextBox", task.Description);
			titleBox.GotFocus += new EventHandler(OnTitleGotFocus);
			titleBox.TextChanged += new EventHandler(OnTitleTextChanged);
			titleBox.KeyDown += new KeyEventHandler(OnTitleKeyDown);
			titleBox.KeyUp += new KeyEventHandler(OnTitleKeyUp);
			titleBox.SizeChanged += new EventHandler(OnTitleSizeChanged);
			this.Controls.Add(titleBox);

			statusComboBox = new StatusComboBox(statusOptions, task.Status);
			statusComboBox.SelectedIndexChanged += new EventHandler(OnStatusSelectedIndexChanged);
			this.Controls.Add(statusComboBox);

			categoryComboBox = new CategoryComboBox(categoryOptions, task.Category);
			categoryComboBox.SelectedIndexChanged += new EventHandler(OnCategorySelectedIndexChanged);
			this.Controls.Add(categoryComboBox);

			Label createLabel = new DataLabel("CreateDate", task.CreateDateString);
			createLabel.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add(createLabel);

			Label doneLabel = new DataLabel("DoneDate", task.DoneDateString);
			doneLabel.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add(doneLabel);

			this.Controls.Add(NewButton(Settings.SYMBOL_MULTIPLY, OnDeleteButtonClicked));

			SetupColumns(this.Controls);
			SetTabIndexes();
		}

		public override void SetRowIndex(int rowIndex)
		{
			base.SetRowIndex(rowIndex);
			indexBox.Text = rowIndex.ToString();
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

		#region Internal Event Handlers

		public void OnIndexGotFocus(object sender, EventArgs e)
		{
			InvokeRowGotFocus();
		}

		public void OnTitleGotFocus(object sender, EventArgs e)
		{
			InvokeRowGotFocus();
		}

		public void OnIndexLostFocus(object sender, EventArgs e)
		{
			int newRow;
			if(Int32.TryParse(indexBox.Text, out newRow))
			{
				if(newRow != rowIndex)
				{
					//if row number changed, move the row
					InvokeIndexChanged(newRow);
					return;
				}
			}
			//if row number is not a valid value, change it back to its previous value
			indexBox.Text = rowIndex.ToString();
		}

		private void OnIndexKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; //stop the error-ding from sounding
			}
		}

		private void OnIndexKeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				FocusOnTitle();
				e.Handled = true;
			}
		}

		public void OnTitleKeyDown(object sender, KeyEventArgs e)
		{
			if(IsControlDownArrow(e) ||
				(IsDownArrow(e) && titleBox.IsCaretOnLastLine()))
			{
				//go to next row
				InvokeGoToRow(rowIndex + 1, titleBox.CaretX, lastLine: false);
				e.Handled = true;
				return;
			}
			if(IsControlUpArrow(e) ||
				(IsUpArrow(e) && titleBox.IsCaretOnFirstLine()))
			{
				//go to previous row
				InvokeGoToRow(rowIndex - 1, titleBox.CaretX, lastLine: true);
				e.Handled = true;
				return;
			}
		}

		private void OnTitleKeyUp(object sender, KeyEventArgs e)
		{
			if(e.Control && e.KeyCode == Keys.N)
			{
				InvokeAddRowBelow();
			}
		}

		private void OnTitleSizeChanged(object sender, EventArgs e)
		{
			TitleTextBox box = (sender as TitleTextBox);
			int newHeight = Math.Max(minHeight, box.Height);
			if(this.Height != newHeight) //don't go into resizing loop
			{
				this.Height = newHeight;
			}
			InvokeRowLocationChanged();
		}

		public void OnTitleTextChanged(object sender, EventArgs e)
		{
			InvokeTitleChanged(titleBox.Text);
		}

		public void OnStatusSelectedIndexChanged(object sender, EventArgs e)
		{
			InvokeStatusChanged(statusComboBox.Text);
		}

		public void OnCategorySelectedIndexChanged(object sender, EventArgs e)
		{
			InvokeCategoryChanged(categoryComboBox.Text);
		}

		public void OnDeleteButtonClicked(object sender, EventArgs e)
		{
			InvokeRowDeleted();
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

		#region Invoke Events

		private void InvokeRowGotFocus()
		{
			if(RowGotFocus == null) return;
			RowGotFocus.Invoke(this, EventArgs.Empty);
		}

		private void InvokeIndexChanged(int newRow)
		{
			if(IndexChanged == null) return;
			IndexChanged.Invoke(this, new IntEventArgs(newRow));
		}

		private void InvokeTitleChanged(string text)
		{
			if(TitleChanged == null) return;
			TitleChanged.Invoke(this, new StringEventArgs(text));
		}

		private void InvokeStatusChanged(string optionText)
		{
			if(StatusChanged == null) return;
			StatusChanged.Invoke(this, new StringEventArgs(optionText));
		}

		private void InvokeCategoryChanged(string optionText)
		{
			if(CategoryChanged == null) return;
			CategoryChanged.Invoke(this, new StringEventArgs(optionText));
		}

		private void InvokeGoToRow(int rowIndex, int caretX, bool lastLine)
		{
			if(GoToRow == null) return;
			GoToRow.Invoke(this, new GoToRowEventArgs(rowIndex, caretX, lastLine));
		}

		private void InvokeRowLocationChanged()
		{
			if(RowLocationChanged == null) return;
			RowLocationChanged.Invoke(this, EventArgs.Empty);
		}

		private void InvokeRowDeleted()
		{
			if(RowDeleted == null) return;
			RowDeleted.Invoke(this, EventArgs.Empty);
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
