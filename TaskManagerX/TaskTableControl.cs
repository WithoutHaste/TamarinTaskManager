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
	public class TaskTableControl : TableLayoutPanel
	{
		public TaskTableToolStrip ToolStrip { get; set; }

		private Project project;
		private History history;

		private bool showActive = true;

		private static Font titleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		private static Font regularFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		private static int? textBoxLineHeight;

		private static int PLUS_COLUMN_INDEX = 0;
		private static int ROW_COLUMN_INDEX = 1;
		private static int ID_COLUMN_INDEX = 2;
		private static int TITLE_COLUMN_INDEX = 3;
		private static int STATUS_COLUMN_INDEX = 4;
		private static int CATEGORY_COLUMN_INDEX = 5;
		private static int CREATED_COLUMN_INDEX = 6;
		private static int DONE_COLUMN_INDEX = 7;
		private static int DELETE_COLUMN_INDEX = 8;
		private static int HEADER_ROW_INDEX = 0;

		public TaskTableControl(Project project)
		{
			this.project = project;
			this.history = new History();

			this.Location = new Point(0, 0);
			this.Padding = new Padding(left: 0, top: 0, right: SystemInformation.VerticalScrollBarWidth, bottom: 0); //leave room for vertical scrollbar
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.Dock = DockStyle.Fill;
			this.BackColor = SystemColors.Control;
			this.AutoScroll = true;

			ShowTaskSheet(active: showActive, forced: true);
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

			this.SuspendLayout(); //avoid screen flickers

			showActive = active;

			this.Controls.Clear();
			this.ColumnStyles.Clear();

			InsertTitleRow();

			int row = 1;
			foreach(Task task in project.GetTasks(active: active))
			{
				InsertTaskRowAt(row, task);
				row++;
			}

			ShowHideTaskIds();
			SetTabIndexes();

			this.ResumeLayout();
		}

		public void ShowHideTaskIds()
		{
			if(Properties.Settings.Default.ShowTaskIds)
				ShowTaskIds();
			else
				HideTaskIds();
		}

		private void ShowTaskIds()
		{
			this.ColumnStyles[ID_COLUMN_INDEX].Width = 45F;
		}

		private void HideTaskIds()
		{
			this.ColumnStyles[ID_COLUMN_INDEX].Width = 10F;
		}

		public void InsertTitleRow()
		{
			this.Controls.Add(NewButton("+", addTask_Click), PLUS_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Row"), ROW_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Id"), ID_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Description"), TITLE_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Status"), STATUS_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Category"), CATEGORY_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Created"), CREATED_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Done"), DONE_COLUMN_INDEX, HEADER_ROW_INDEX);

			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (showActive ? 0F : 80F)));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));

			this.ColumnCount = DELETE_COLUMN_INDEX + 1;
			this.RowCount = HEADER_ROW_INDEX + 1;
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
			for(int row = 1; row <= this.RowCount; row++)
			{
				Control control = this.GetControlFromPosition(STATUS_COLUMN_INDEX, row);
				if(!(control is ComboBox))
					continue;
				ComboBox newComboBox = GenerateStatusComboBox((control as ComboBox).Text);
				this.Controls.Remove(control);
				this.Controls.Add(newComboBox, STATUS_COLUMN_INDEX, row);
			}
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
			}
		}

		private void UpdateCategoryComboBoxOptions()
		{
			for(int row = 1; row <= this.RowCount; row++)
			{
				Control control = this.GetControlFromPosition(CATEGORY_COLUMN_INDEX, row);
				if(!(control is ComboBox))
					continue;
				ComboBox newComboBox = GenerateCategoryComboBox((control as ComboBox).Text);
				this.Controls.Remove(control);
				this.Controls.Add(newComboBox, CATEGORY_COLUMN_INDEX, row);
			}
		}

		private void InsertTaskRowAt(int rowIndex, Task task)
		{
			this.SuspendLayout(); //avoid screen flickers

			InsertRowAt(rowIndex);

			this.Controls.Add(NewButton("+", addTask_Click), PLUS_COLUMN_INDEX, rowIndex);

			TextBox rowNumberBox = NewTextBox("RowNumberTextBox", rowIndex.ToString());
			rowNumberBox.LostFocus += new EventHandler(rowNumberTextBox_LostFocus);
			rowNumberBox.TabStop = false;
			this.Controls.Add(rowNumberBox, ROW_COLUMN_INDEX, rowIndex);

			this.Controls.Add(NewDataLabel("Id", task.Id.ToString()), ID_COLUMN_INDEX, rowIndex);

			RichTextBox titleBox = NewRichTextBox("TitleTextBox", task.Title);
			titleBox.TextChanged += new EventHandler(titleTextBox_TextChanged);
			//titleBox.SizeChanged += new EventHandler(titleTextBox_SizeChanged);
			titleBox.TabIndex = 1;
			this.Controls.Add(titleBox, TITLE_COLUMN_INDEX, rowIndex);

			ComboBox statusComboBox = GenerateStatusComboBox(task.Status);
			statusComboBox.TabIndex = 2;
			this.Controls.Add(statusComboBox, STATUS_COLUMN_INDEX, rowIndex);

			ComboBox categoryComboBox = GenerateCategoryComboBox(task.Category);
			categoryComboBox.TabIndex = 3;
			this.Controls.Add(categoryComboBox, CATEGORY_COLUMN_INDEX, rowIndex);

			this.Controls.Add(NewDataLabel("CreateDate", task.CreateDateString), CREATED_COLUMN_INDEX, rowIndex);
			this.Controls.Add(NewDataLabel("DoneDate", task.DoneDateString), DONE_COLUMN_INDEX, rowIndex);

			this.Controls.Add(NewButton("X", deleteTask_Click), DELETE_COLUMN_INDEX, rowIndex);

			this.RowCount++;

			SetTabIndexes();

			this.ResumeLayout();
		}

		private ComboBox GenerateStatusComboBox(string selectedOption)
		{
			List<string> options = project.Statuses.ToList();
			if(!options.Contains(selectedOption))
				options.Add(selectedOption);

			ComboBox statusComboBox = NewComboBox("StatusComboBox", options.ToArray(), selectedOption);
			statusComboBox.SelectedIndexChanged += new EventHandler(statusComboBox_SelectedIndexChanged);
			return statusComboBox;
		}

		private ComboBox GenerateCategoryComboBox(string selectedOption)
		{
			List<string> options = project.Categories.ToList();
			if(!options.Contains(selectedOption))
				options.Add(selectedOption);

			ComboBox categoryComboBox = NewComboBox("CategoryComboBox", options.ToArray(), selectedOption);
			categoryComboBox.SelectedIndexChanged += new EventHandler(categoryComboBox_SelectedIndexChanged);
			return categoryComboBox;
		}

		private void InsertRowAt(int rowIndex)
		{
			foreach(Control control in this.Controls)
			{
				int controlRow = this.GetRow(control);
				if(controlRow < rowIndex)
					continue;

				this.SetRow(control, controlRow + 1);

				if(control.Name == "RowNumberTextBox")
				{
					(control as TextBox).Text = (Int32.Parse((control as TextBox).Text) + 1).ToString();
				}
			}
			SetTabIndexes();
		}

		private void RemoveRow(int rowIndex)
		{
			this.SuspendLayout(); //avoid screen flickers

			for(int col = 0; col < this.ColumnCount; col++)
			{
				Control control = this.GetControlFromPosition(col, rowIndex);
				this.Controls.Remove(control);
			}
			foreach(Control control in this.Controls)
			{
				int controlRow = this.GetRow(control);
				if(controlRow < rowIndex)
					continue;

				this.SetRow(control, controlRow - 1);

				if(control.Name == "RowNumberTextBox")
				{
					(control as TextBox).Text = (Int32.Parse((control as TextBox).Text) - 1).ToString();
				}
			}
			SetTabIndexes();

			this.ResumeLayout();
		}

		private void MoveRow(int fromRow, int toRow)
		{
			if(fromRow == toRow)
				return;
			Task task = project.MoveRow(IndexConverter.TableLayoutPanelToExcelWorksheet(fromRow), IndexConverter.TableLayoutPanelToExcelWorksheet(toRow), showActive);
			RemoveRow(fromRow);
			//going down, push toRow up - already done by removing task
			//going up, push toRow down
			InsertTaskRowAt(toRow, task);
			FocusOnTitle(toRow);
		}

		private void FocusOnTitle(int row, int caret = -1, int selectionLength = 0)
		{
			Control control = this.GetControlFromPosition(TITLE_COLUMN_INDEX, row);
			if(control == null)
				return;

			control.Focus();
			if(control is RichTextBox)
			{
				RichTextBox textBox = (control as RichTextBox);
				if(caret == -1)
				{
					caret = textBox.Text.Length;
					selectionLength = 0;
				}
				textBox.Select(caret, selectionLength);
			}
		}

		private void addTask_Click(object sender, EventArgs e)
		{
			//add task below current
			int row = this.GetRow(sender as Control) + 1;
			InsertTaskRowAt(row, project.InsertNewTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive));
			history.Add(new AddAction(showActive, row));
		}

		public void ManualAddTask(bool activeSheet, int row, Task task = null)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			if(task == null)
				task = project.InsertNewTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
			else
				project.InsertTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive, task: task);
			InsertTaskRowAt(row, task);
			history.On();
		}

		private void deleteTask_Click(object sender, EventArgs e)
		{
			int row = this.GetRow(sender as Control);
			Task task = project.GetTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
			project.RemoveTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
			RemoveRow(row);
			history.Add(new DeleteAction(showActive, row, task));
		}

		public void ManualDeleteTask(bool activeSheet, int row)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			project.RemoveTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
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

		public void ManualMoveTask(bool activeSheet, int fromRowNumber, int toRowNumber)
		{
			history.Off();
			ToolStrip.SelectActiveInactive(activeSheet);
			MoveRow(fromRowNumber, toRowNumber);
			history.On();
		}

		private void titleTextBox_TextChanged(object sender, EventArgs e)
		{
			RichTextBox textBox = (sender as RichTextBox);
			int row = this.GetRow(textBox);
			string previousText = project.GetTitle(IndexConverter.TableLayoutPanelToExcelWorksheet(row), showActive);
			project.UpdateTitle(IndexConverter.TableLayoutPanelToExcelWorksheet(row), textBox.Text, active: showActive);
			SetTextBoxHeightByText(textBox);
			history.Add(new TextAction(showActive, row, previousText, textBox.Text));
		}

		private void titleTextBox_SizeChanged(object sender, EventArgs e)
		{
			RichTextBox textBox = (sender as RichTextBox);
			SetTextBoxHeightByText(textBox);
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
			string previousStatus = project.GetStatus(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
			if(previousStatus == comboBox.Text)
				return;			

			ChangeStatusAction historyAction = new ChangeStatusAction(showActive, row, previousStatus);
			StatusChangeResult result = project.UpdateStatus(IndexConverter.TableLayoutPanelToExcelWorksheet(row), comboBox.Text, active: showActive);
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
		}

		private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);
			string previousCategory = project.GetCategory(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
			if(previousCategory == comboBox.Text)
				return;

			project.UpdateCategory(IndexConverter.TableLayoutPanelToExcelWorksheet(row), comboBox.Text, active: showActive);
			history.Add(new ChangeCategoryAction(showActive, row, previousCategory, comboBox.Text));
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
			this.SuspendLayout(); //avoid screen flickers
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
			this.ResumeLayout();
		}

		private Label NewTitleLabel(string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = titleFont;
			label.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			label.Size = new System.Drawing.Size(21, 24);
			label.Text = text;
			return label;
		}

		private Label NewDataLabel(string name, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = regularFont;
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
			textBox.Font = regularFont;
			textBox.Name = name;
			textBox.Text = text;
			textBox.Size = new System.Drawing.Size(119, 22);
			return textBox;
		}

		private RichTextBox NewRichTextBox(string name, string text = null)
		{
			RichTextBox textBox = new RichTextBox();
			if(textBoxLineHeight == null)
			{
				using(Graphics g = textBox.CreateGraphics())
				{
					textBoxLineHeight = TextRenderer.MeasureText(g, "TEST", regularFont).Height;
				}
			}

			textBox.Dock = System.Windows.Forms.DockStyle.Top;
			textBox.Font = regularFont;
			textBox.Name = name;
			textBox.Width = 119;
			textBox.Text = text;
			SetTextBoxHeightByText(textBox);
			return textBox;
		}

		private void SetTextBoxHeightByText(RichTextBox textBox)
		{
			int newHeight = 5 + (textBoxLineHeight.Value * CountLines(textBox));
			if(newHeight == textBox.Size.Height)
				return; //do not go into a resizing loop
			textBox.Size = new System.Drawing.Size(textBox.Width, newHeight);
		}

		private ComboBox NewComboBox(string name, string[] options, string selectedValue = null)
		{
			ComboBox comboBox = new ComboBox();
			comboBox.Font = regularFont;
			comboBox.FormattingEnabled = true;
			comboBox.Name = name;
			foreach(string option in options) //when using datasource, could not set the selected item
			{
				comboBox.Items.Add(option);
			}
			if(!String.IsNullOrEmpty(selectedValue))
			{
				comboBox.SelectedIndex = comboBox.FindString(selectedValue);
			}
			comboBox.Size = new System.Drawing.Size(94, 24);
			return comboBox;
		}

		private Button NewButton(string text, EventHandler onClickHandler)
		{
			Button button = new Button();
			button.Font = regularFont;
			button.Location = new System.Drawing.Point(3, 3);
			button.Size = new System.Drawing.Size(19, 23);
			button.TabStop = false;
			button.Text = text;
			button.UseVisualStyleBackColor = true;
			button.Click += onClickHandler;
			return button;
		}

		private int CountLines(RichTextBox textBox)
		{
			int lineCount = 1;
			while(textBox.GetFirstCharIndexFromLine(lineCount) > -1)
			{
				lineCount++;
			}
			return lineCount;
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
	}
}
