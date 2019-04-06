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
	public class TaskTableControl : Panel
	{
		public event ShowColumnEventHandler ShowColumn;
		public event ListEventHandler StatusesChanged;
		public event ListEventHandler CategoriesChanged;

		public TaskTableToolStrip ToolStrip { get; set; }

		private Project project;
		private History history;

		private bool showActive = true;

		private bool DisplayCategories {
			get {
				return (project.Categories.Length > 1);
			}
		}

		public TaskTableControl(Project project)
		{
			this.project = project;
			this.history = new History();

			this.Location = new Point(0, 0);
			this.Padding = new Padding(left: 0, top: 0, right: SystemInformation.VerticalScrollBarWidth, bottom: 0); //leave room for vertical scrollbar
			this.Dock = DockStyle.Fill;
			this.BackColor = Color.White;
			this.AutoScroll = true;
			this.VisibleChanged += new EventHandler(taskTableControl_VisibleChanged);

			ShowTaskSheet(active: showActive, forced: true);
		}

		private void taskTableControl_VisibleChanged(object sender, EventArgs e)
		{
			if(!(sender as Control).Visible)
				return;
			CheckForOutsideEdits();
		}

		public void CheckForOutsideEdits()
		{
			if(project.EditedByOutsideSource)
			{
				DialogResult result = MessageBox.Show(project.Name + " has been edited by an outside source. Reload?\nYou will lose any changed since your last save.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if(result == DialogResult.Yes)
				{
					ReloadProject();
				}
			}
		}

		private void ReloadProject()
		{
			history.Clear();
			project.ReloadProject();
			ShowTaskSheet(showActive, forced:true);
		}

		public void Undo()
		{
			if(!history.CanUndo)
				return;
			HistoryAction action = history.Undo();
			action.Undo(this);
		}

		public void Redo()
		{
			if(!history.CanRedo)
				return;
			HistoryAction action = history.Redo();
			action.Redo(this);
		}

		public void ShowTaskSheet(bool active, bool forced = false)
		{
			if(!forced && showActive == active)
				return;

			showActive = active;

			this.Controls.Clear();

			InsertTitleRow();

			int row = 1;
			foreach(Task task in project.GetTasks(active: showActive))
			{
				InsertTaskRowAt(row, task);
				row++;
			}

			ShowHideTaskIds();
			ShowHideCategories();
			SetTabIndexes();
		}

		public void ShowHideTaskIds()
		{
			try
			{
				if(Properties.Settings.Default.ShowTaskIds)
					ShowTaskIds();
				else
					HideTaskIds();
			}
			catch
			{
				HideTaskIds();
			}
		}

		private void ShowTaskIds()
		{
			Column_Show(true, TamarinRowControl.ID_COLUMN_INDEX);
		}

		private void HideTaskIds()
		{
			Column_Show(false, TamarinRowControl.ID_COLUMN_INDEX);
		}

		public void ShowHideCategories()
		{
			if(DisplayCategories)
				ShowCategories();
			else
				HideCategories();
		}

		private void ShowCategories()
		{
			Column_Show(true, TamarinRowControl.CATEGORY_COLUMN_INDEX);
		}

		private void HideCategories()
		{
			Column_Show(false, TamarinRowControl.CATEGORY_COLUMN_INDEX);
		}

		private void Column_Show(bool show, int columnIndex)
		{
			if(ShowColumn == null) return;
			ShowColumn.Invoke(this, new ShowColumnEventArgs(show, columnIndex));
		}

		public void InsertTitleRow()
		{
			HeaderRowControl row = new HeaderRowControl();
			row.Location = new Point(0, 0);
			this.Controls.Add(row);
		}

		public void EditStatuses()
		{
			using(StatusForm statusForm = new StatusForm(project.ActiveStatuses, project.InactiveStatuses))
			{
				DialogResult result = statusForm.ShowDialog();
				if(result != DialogResult.OK)
					return;
				try
				{
					project.SetStatuses(statusForm.GetActiveStatuses(), statusForm.GetInactiveStatuses());
				}
				catch(Exception e)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				UpdateStatusComboBoxOptions();
			}
		}

		private void UpdateStatusComboBoxOptions()
		{
			if(StatusesChanged == null) return;
			StatusesChanged.Invoke(this, new ListEventArgs(project.Statuses));
		}

		public void EditCategories()
		{
			using(CategoryForm categoryForm = new CategoryForm(project.Categories))
			{
				DialogResult result = categoryForm.ShowDialog();
				if(result != DialogResult.OK)
					return;
				try
				{
					project.Categories = categoryForm.GetCategories();
				}
				catch(Exception e)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				UpdateCategoryComboBoxOptions();
				ShowHideCategories();
			}
		}

		private void UpdateCategoryComboBoxOptions()
		{
			if(CategoriesChanged == null) return;
			CategoriesChanged.Invoke(this, new ListEventArgs(project.Categories));
		}

		private void InsertTaskRowAt(int rowIndex, Task task)
		{
			TaskRowControl row = new TaskRowControl(rowIndex, task, project.Statuses.ToList(), project.Categories.ToList());

			int maxY = 0;
			foreach(Control c in this.Controls)
			{
				maxY = Math.Max(maxY, c.Bottom);
			}
			row.Location = new Point(0, maxY);

			this.Controls.Add(row);
		}

		private void AdjustRowIndexesAndPositions()
		{
			int index = 0;
			int maxY = 0;
			foreach(TamarinRowControl c in this.Controls)
			{
				c.SetRowIndex(index);
				c.Location = new Point(0, maxY);
				index++;
				maxY = c.Bottom;
			}
		}

		private void InsertRowAt(int rowIndex)
		{
			TaskRowControl row = new TaskRowControl(rowIndex, new Task(), project.Statuses.ToList(), project.Categories.ToList());
			this.Controls.Add(row);
			this.Controls.SetChildIndex(row, rowIndex);
			AdjustRowIndexesAndPositions();
		}

		private void RemoveRow(int rowIndex)
		{
			int i = 0;
			foreach(Control c in this.Controls)
			{
				if(i == rowIndex)
				{
					this.Controls.Remove(c);
					AdjustRowIndexesAndPositions();
					return;
				}
				i++;
			}
		}

		private TamarinRowControl GetRowByIndex(int index)
		{
			int i = 0;
			foreach(TamarinRowControl row in this.Controls)
			{
				if(i == index)
					return row;
				i++;
			}
			return null;
		}

		private void MoveRow(int fromRow, int toRow)
		{
			if(fromRow == toRow)
				return;
			Task task = project.MoveRow(fromRow, toRow, showActive);
			TamarinRowControl row = GetRowByIndex(fromRow);
			this.Controls.SetChildIndex(row, toRow);
			if(row is TaskRowControl)
				(row as TaskRowControl).FocusOnTitle();
			AdjustRowIndexesAndPositions();
		}

		private void FocusOnTitle(int row, int caret = -1, int selectionLength = 0)
		{
			Control control = this.GetControlFromPosition(TITLE_COLUMN_INDEX, row);
			if(control == null || !(control is RichTextBox))
				return;

			control.Focus();
			if(control is RichTextBox)
			{
				RichTextBox textBox = (control as RichTextBox);
				if(caret == -1)
				{
					caret = 0;
					selectionLength = 0;
				}
				textBox.Select(caret, selectionLength);
			}
		}

		private void SelectTitleTextBox(int fromRow, int toRow)
		{
			if(toRow <= 0)
				return;
			RichTextBox previousTextBox = (RichTextBox)this.GetControlFromPosition(column: TITLE_COLUMN_INDEX, row: fromRow);
			int caret = previousTextBox.SelectionStart;
			if(fromRow < toRow)
			{
				caret = 0;
			}
			FocusOnTitle(toRow, caret);
		}

		private void addTask_Click(object sender, EventArgs e)
		{
			//add task below current
			int row = this.GetRow(sender as Control) + 1;
			InsertTaskRowAt(row, project.InsertNewTask(row, active: showActive));
			history.Add(new AddAction(showActive, row));
			FocusOnTitle(row);
		}

		public void ManualAddTask(bool activeSheet, int row, Task task = null)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			if(task == null)
				task = project.InsertNewTask(row, active: showActive);
			else
				project.InsertTask(row, active: showActive, task: task);
			InsertTaskRowAt(row, task);
			history.On();
		}

		private void deleteTask_Click(object sender, EventArgs e)
		{
			int row = this.GetRow(sender as Control);
			Task task = project.GetTask(row, active: showActive);
			project.RemoveTask(row, active: showActive);
			RemoveRow(row);
			history.Add(new DeleteAction(showActive, row, task));
		}

		public void ManualDeleteTask(bool activeSheet, int row)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			project.RemoveTask(row, active: showActive);
			RemoveRow(row);
			history.On();
		}

		private void rowNumberTextBox_LostFocus(object sender, EventArgs e)
		{
			TextBox textBox = (sender as TextBox);
			int row = this.GetRow(textBox);
			int newRow;
			if(!Int32.TryParse(textBox.Text, out newRow))
			{
				textBox.Text = row.ToString();
				return;
			}
			MoveRow(row, newRow);
			history.Add(new MoveAction(showActive, row, newRow));
		}

		private void rowNumberTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; //stop the error-ding from sounding
			}
		}

		private void rowNumberTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			TextBox textBox = (sender as TextBox);
			int row = this.GetRow(textBox);

			if(e.KeyCode == Keys.Enter)
			{
				this.GetControlFromPosition(column: TITLE_COLUMN_INDEX, row: row).Focus(); //lose focus here to trigger move event
				e.Handled = true;
			}
		}

		private void titleTextBox_GotFocus(object sender, EventArgs e)
		{
			int row = this.GetRow(sender as Control);
			if(row == 1)
			{
				this.ScrollControlIntoView(this.GetControlFromPosition(column: PLUS_COLUMN_INDEX, row: 0));
			}
			else
			{
				this.ScrollControlIntoView(sender as Control);
			}
		}

		private void titleTextBox_TextChanged(object sender, EventArgs e)
		{
			RichTextBox textBox = (sender as RichTextBox);
			int row = this.GetRow(textBox);
			string previousText = project.GetTitle(row, showActive);
			project.UpdateTitle(row, textBox.Text, active: showActive);
			history.Add(new TextAction(showActive, row, previousText, textBox.Text));
		}

		private void titleTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			TitleTextBox textBox = (sender as TitleTextBox);
			if(e.KeyCode == Keys.Down)
			{
				if(e.Control)
				{
					//move to beginning of next textbox
					int row = this.GetRow(sender as Control);
					FocusOnTitle(row + 1);
					e.Handled = true;
				}
				else
				{
					//if cursor is on last line, move to next textbox
					if(textBox.CursorOnLastLine())
					{
						int row = this.GetRow(sender as Control);
						SelectTitleTextBox(row, row + 1);
						e.Handled = true;
					}
				}
			}
			if(e.KeyCode == Keys.Up)
			{
				if(e.Control)
				{
					//move to beginning of next textbox
					int row = this.GetRow(sender as Control);
					FocusOnTitle(row - 1);
					e.Handled = true;
				}
				else
				{
					//if cursor is on first line, move to previous textbox
					if(textBox.CursorOnFirstLine())
					{
						int row = this.GetRow(sender as Control);
						SelectTitleTextBox(row, row - 1);
						e.Handled = true;
					}
				}
			}
		}

		private void titleTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Control && e.KeyCode == Keys.N)
			{
				addTask_Click(sender, e);
			}
		}

		public void ManualMoveTask(bool activeSheet, int fromRowNumber, int toRowNumber)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			MoveRow(fromRowNumber, toRowNumber);
			history.On();
		}

		public void ManualTextChange(bool activeSheet, int row, string text, int caret, int selectionLength)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			Control control = this.GetControlFromPosition(TITLE_COLUMN_INDEX, row);
			(control as RichTextBox).Text = text;
			FocusOnTitle(row, caret, selectionLength);
			history.On();
		}

		private void statusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);
			string previousStatus = project.GetStatus(row, active: showActive);
			if(previousStatus == comboBox.Text)
				return;			

			ChangeStatusAction historyAction = new ChangeStatusAction(showActive, row, previousStatus);
			StatusChangeResult result = project.UpdateStatus(row, comboBox.Text, active: showActive);
			Label dateDoneLabel = (Label)this.GetControlFromPosition(7, row);
			dateDoneLabel.Text = result.DoneDateString;

			if(result.ActiveInactiveChanged)
			{
				historyAction.SetNew(!showActive, 1, comboBox.Text);
				RemoveRow(row);
			}
			else
			{
				historyAction.SetNew(showActive, row, comboBox.Text);
			}
			history.Add(historyAction);
			FocusOnTitle(row);
		}

		private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);
			string previousCategory = project.GetCategory(row, active: showActive);
			if(previousCategory == comboBox.Text)
				return;

			project.UpdateCategory(row, comboBox.Text, active: showActive);
			history.Add(new ChangeCategoryAction(showActive, row, previousCategory, comboBox.Text));
			FocusOnTitle(row);
		}

		private void comboBox_MouseWheel(object sender, MouseEventArgs e)
		{
			(e as HandledMouseEventArgs).Handled = true;
		}

		public void ManualChangeTaskCategory(bool activeSheet, int row, string category)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			ComboBox comboBox = this.GetControlFromPosition(CATEGORY_COLUMN_INDEX, row) as ComboBox;
			if(!comboBox.Items.Contains(category))
				comboBox.Items.Add(category);
			comboBox.SelectedIndex = comboBox.Items.IndexOf(category);
			history.On();
		}

		public void ManualChangeTaskStatus(bool currentActiveSheet, int currentRow, bool finalActiveSheet, int finalRow, string status)
		{
			RequestSuspendLayout();
			history.Off();
			ToolStrip.SelectActiveInactive(currentActiveSheet);
			ComboBox comboBox = this.GetControlFromPosition(STATUS_COLUMN_INDEX, currentRow) as ComboBox;
			if(!comboBox.Items.Contains(status))
				comboBox.Items.Add(status);
			comboBox.SelectedIndex = comboBox.Items.IndexOf(status);

			if(currentActiveSheet != finalActiveSheet || currentRow != finalRow)
			{
				ToolStrip.SelectActiveInactive(finalActiveSheet);
				MoveRow(1, finalRow);
			}

			history.On();
			RequestResumeLayout();
		}

		private void SetTabIndexes()
		{
			for(int row = 1; row < this.RowCount; row++)
			{
				Control titleControl = this.GetControlFromPosition(TITLE_COLUMN_INDEX, row);
				if(titleControl == null)
					continue;
				titleControl.TabIndex = (row*10) + 1;

				Control statusControl = this.GetControlFromPosition(STATUS_COLUMN_INDEX, row);
				statusControl.TabIndex = (row*10) + 2;

				Control categoryControl = this.GetControlFromPosition(CATEGORY_COLUMN_INDEX, row);
				categoryControl.TabIndex = (row*10) + 3;
			}
		}

		public void ClearAllInactive()
		{
			if(showActive)
			{
				MessageBox.Show("Cannot delete inactive items while active items are displayed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			MultipleAction multipleAction = new MultipleAction();
			int row = 1;
			while(this.RowCount > 1)
			{
				Task task = project.GetTask(row, active: showActive);
				project.RemoveTask(row, active: showActive);
				RemoveRow(row);
				multipleAction.AddAction(new DeleteAction(showActive, row, task));
			}
			history.Add(multipleAction);
		}
	}
}
