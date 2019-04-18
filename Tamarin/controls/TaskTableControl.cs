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
		public event EventHandler<ShowColumnEventArgs> ShowColumn;
		public event EventHandler<ListEventArgs> StatusesChanged;
		public event EventHandler<ListEventArgs> CategoriesChanged;

		public TaskTableToolStrip ToolStrip { get; set; }

		private Project project;
		private History history;

		/// <summary>True when displayed Active tasks, False when displaying Inactive tasks.</summary>
		private bool showActive = true;
		/// <summary>Set to true the first time the panel is displayed.</summary>
		private bool shown = false;

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
			this.VisibleChanged += new EventHandler(OnVisibleChanged);
			this.SizeChanged += new EventHandler(OnSizeChanged);

			ShowTaskSheet(active: showActive, forced: true);
		}

		#region Event Handlers

		private void OnVisibleChanged(object sender, EventArgs e)
		{
			if(!(sender as Control).Visible)
				return;
			CheckForOutsideEdits();
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			RecalculateColumnWidths();
		}

		private void OnAddRowBelow(object sender, EventArgs e)
		{
			TamarinRowControl row = (sender as TamarinRowControl);
			int nextRowIndex = row.RowIndex + 1;
			Task newTask = InsertTaskRowAt(nextRowIndex, project.InsertNewTask(nextRowIndex, active: showActive));
			history.Add(new AddAction(showActive, nextRowIndex, newTask.Id));
			FocusOnTitleByIndex(nextRowIndex);
		}

		private void OnRowLocationChanged(object sender, EventArgs e)
		{
			//adjust all row positions from top to bottom
			int y = 0;
			foreach(Control c in this.Controls)
			{
				if(c.Top != y)
					c.Top = y;
				y = c.Bottom;
			}
		}

		private void OnRowDeleted(object sender, EventArgs e)
		{
			TaskRowControl row = (sender as TaskRowControl);
			Task task = project.GetTask(row.RowIndex, active: showActive);
			project.RemoveTask(row.RowIndex, active: showActive);
			RemoveRow(row.RowIndex);
			history.Add(new DeleteAction(showActive, row.RowIndex, task.Id, task));
			FocusOnPreviousTitleByIndex(row.RowIndex);
		}

		private void OnMoveRow(object sender, IntEventArgs e)
		{
			TaskRowControl row = (sender as TaskRowControl);
			MoveRow(row.RowIndex, e.Value);
			history.Add(new MoveAction(showActive, row.RowIndex, e.Value));
		}

		private void OnRowGotFocus(object sender, EventArgs e)
		{
			this.ScrollControlIntoView(sender as Control);
		}

		private void OnRowTitleChanged(object sender, StringEventArgs e)
		{
			TaskRowControl row = (sender as TaskRowControl);
			string previousText = project.GetTitle(row.RowIndex, showActive);
			project.UpdateTitle(row.RowIndex, e.Value, active: showActive);
			history.Add(new TextAction(showActive, row.RowIndex, previousText, e.Value));
		}

		private void OnGoToRow(object sender, GoToRowEventArgs e)
		{
			TamarinRowControl row = GetRowByIndex(e.RowIndex);
			if(row == null || !(row is TaskRowControl))
				return;
			(row as TaskRowControl).FocusOnTitle(e.CaretX, e.LastLine);
		}

		private void OnRowStatusChanged(object sender, StringEventArgs e)
		{
			TaskRowControl row = (sender as TaskRowControl);
			string previousStatus = project.GetStatus(row.RowIndex, active: showActive);
			if(previousStatus == e.Value)
				return;

			ChangeStatusAction historyAction = new ChangeStatusAction(showActive, row.RowIndex, previousStatus);
			StatusChangeResult result = project.UpdateStatus(row.RowIndex, e.Value, active: showActive);

			if(result.ActiveInactiveChanged)
			{
				historyAction.SetNew(!showActive, 1, e.Value);
				RemoveRow(row.RowIndex);
			}
			else
			{
				historyAction.SetNew(showActive, row.RowIndex, e.Value);
			}
			history.Add(historyAction);
			row.FocusOnTitle();
		}

		private void OnRowCategoryChanged(object sender, StringEventArgs e)
		{
			TaskRowControl row = (sender as TaskRowControl);
			string previousCategory = project.GetCategory(row.RowIndex, active: showActive);
			if(previousCategory == e.Value)
				return;

			project.UpdateCategory(row.RowIndex, e.Value, active: showActive);
			history.Add(new ChangeCategoryAction(showActive, row.RowIndex, previousCategory, e.Value));
			row.FocusOnTitle();
		}

		private void OnComboBoxMouseWheel(object sender, MouseEventArgs e)
		{
			//TODO used anywhere?
			(e as HandledMouseEventArgs).Handled = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if(!shown) //because Panel does not have OnShow event
			{
				FocusOnTitleByIndex(1); //focus on first data row when panel is first displayed
				shown = true;
			}
		}

		#endregion

		#region History

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

		public void ManualAddTask(bool activeSheet, int rowIndex, Task task = null)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			if(task == null)
				task = project.InsertNewTask(rowIndex, active: showActive);
			else
				project.InsertTask(rowIndex, active: showActive, task: task);
			InsertTaskRowAt(rowIndex, task);
			history.On();
			FocusOnTitleByIndex(rowIndex);
		}

		public void ManualDeleteTask(bool activeSheet, int rowIndex, int id)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			project.RemoveTask(rowIndex, active: showActive);
			project.UnUseId(id);
			RemoveRow(rowIndex);
			history.On();
			FocusOnPreviousTitleByIndex(rowIndex);
		}

		public void ManualMoveTask(bool activeSheet, int fromRowNumber, int toRowNumber)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			MoveRow(fromRowNumber, toRowNumber);
			history.On();
		}

		public void ManualTextChange(bool activeSheet, int rowIndex, string text, int caret, int selectionLength)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			TamarinRowControl row = GetRowByIndex(rowIndex);
			if(row == null || !(row is TaskRowControl))
				return;
			(row as TaskRowControl).SetTitle(text);
			(row as TaskRowControl).FocusOnTitle(caret, selectionLength);
			history.On();
		}

		public void ManualChangeTaskCategory(bool activeSheet, int rowIndex, string category)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			TamarinRowControl row = GetRowByIndex(rowIndex);
			if(row == null || !(row is TaskRowControl))
				return;
			(row as TaskRowControl).SetCategory(category);
			history.On();
		}

		public void ManualChangeTaskStatus(bool currentActiveSheet, int currentRowIndex, bool finalActiveSheet, int finalRowIndex, string status)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(currentActiveSheet);
			TamarinRowControl row = GetRowByIndex(currentRowIndex);
			if(row == null || !(row is TaskRowControl))
				return;
			(row as TaskRowControl).SetStatus(status);

			if(currentActiveSheet != finalActiveSheet || currentRowIndex != finalRowIndex)
			{
				ToolStrip.SelectActiveInactive(finalActiveSheet);
				MoveRow(1, finalRowIndex);
			}
			history.On();
		}

		#endregion

		public void InsertTitleRow()
		{
			HeaderRowControl row = new HeaderRowControl();
			row.AddRowBelow += new EventHandler(OnAddRowBelow);
			row.Location = new Point(0, 0);
			row.Width = this.ClientRectangle.Width;
			row.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.Controls.Add(row);
		}

		private Task InsertTaskRowAt(int rowIndex, Task task = null)
		{
			if(task == null)
				task = new Task();

			TaskRowControl row = new TaskRowControl(rowIndex, task, project.Statuses.ToList(), project.Categories.ToList());
			row.AddRowBelow += new EventHandler(OnAddRowBelow);
			row.RowLocationChanged += new EventHandler(OnRowLocationChanged);
			row.IndexChanged += new EventHandler<IntEventArgs>(OnMoveRow);
			row.TitleChanged += new EventHandler<StringEventArgs>(OnRowTitleChanged);
			row.GoToRow += new EventHandler<GoToRowEventArgs>(OnGoToRow);
			row.StatusChanged += new EventHandler<StringEventArgs>(OnRowStatusChanged);
			row.CategoryChanged += new EventHandler<StringEventArgs>(OnRowCategoryChanged);
			row.RowDeleted += new EventHandler(OnRowDeleted);

			row.Location = new Point(0, GetControlsMaxY());
			row.Width = this.ClientRectangle.Width;
			row.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.Controls.Add(row);
			this.Controls.SetChildIndex(row, rowIndex);
			AdjustRowIndexesAndPositions();

			return task;
		}

		private void RemoveRow(int rowIndex)
		{
			TamarinRowControl row = GetRowByIndex(rowIndex);
			if(!(row is TaskRowControl))
				return;
			this.Controls.Remove(row);
			AdjustRowIndexesAndPositions();
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

		/// <summary>
		/// Give focus to TitleBox in selected row.
		/// </summary>
		/// <remarks>
		/// Does not throw exceptions.
		/// </remarks>
		private void FocusOnTitleByIndex(int index)
		{
			TamarinRowControl row = GetRowByIndex(index);
			if(row != null && row is TaskRowControl)
				(row as TaskRowControl).FocusOnTitle();
		}

		/// <summary>
		/// Used when a row is removed.
		/// If there is a previous row, focus on it.
		/// If there is not, focus on the next (now current) row instead.
		/// </summary>
		/// <param name='index'>Index of the deleted row.</param>
		private void FocusOnPreviousTitleByIndex(int index)
		{
			if(index > 1)
				FocusOnTitleByIndex(index - 1);
			else
				FocusOnTitleByIndex(index);
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

		private void RecalculateColumnWidths()
		{
/*			if(this.Width <= 10)
				return;
			if(columnWidths == null)
			{
				columnWidths = new int[TamarinRowControl.DELETE_COLUMN_INDEX + 1];
			}
			int half = this.Width / 2;
			columnWidths[TamarinRowControl.PLUS_COLUMN_INDEX] = Math.Min(20, (int)(half * 0.1));
			columnWidths[TamarinRowControl.ROW_COLUMN_INDEX] = 20;
			columnWidths[TamarinRowControl.ID_COLUMN_INDEX] = 20;
			columnWidths[TamarinRowControl.TITLE_COLUMN_INDEX] = 20;
			columnWidths[TamarinRowControl.STATUS_COLUMN_INDEX] = 40;
			columnWidths[TamarinRowControl.CATEGORY_COLUMN_INDEX] = 40;
			columnWidths[TamarinRowControl.CREATED_COLUMN_INDEX] = 40;
			columnWidths[TamarinRowControl.DONE_COLUMN_INDEX] = 40;
			columnWidths[TamarinRowControl.DELETE_COLUMN_INDEX] = 40;*/
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

		private int GetControlsMaxY()
		{
			int maxY = 0;
			foreach(Control c in this.Controls)
			{
				maxY = Math.Max(maxY, c.Bottom);
			}
			return maxY;
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

		public void ClearAllInactive()
		{
			if(showActive)
			{
				MessageBox.Show("Cannot delete inactive items while active items are displayed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			MultipleAction multipleAction = new MultipleAction();
			int rowIndex = 1;
			while(this.Controls.Count > 1)
			{
				Task task = project.GetTask(rowIndex, active: showActive);
				project.RemoveTask(rowIndex, active: showActive);
				RemoveRow(rowIndex);
				multipleAction.AddAction(new DeleteAction(showActive, rowIndex, task.Id, task));
			}
			history.Add(multipleAction);
		}
	}
}
