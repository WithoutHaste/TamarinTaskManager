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
		private Project project;

		private bool showActive = true;

		private static Font titleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		private static Font regularFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

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

			this.Location = new Point(0, 0);
			this.Padding = new Padding(left: 0, top: 0, right: SystemInformation.VerticalScrollBarWidth, bottom: 0); //leave room for vertical scrollbar
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.Dock = DockStyle.Fill;
			this.BackColor = SystemColors.Control;
			this.AutoScroll = true;

			ShowTaskSheet(active: showActive);
		}

		public void ShowTaskSheet(bool active)
		{
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
		}

		public void InsertTitleRow()
		{
			this.Controls.Add(NewButton("+", addTask_Click), PLUS_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Row"), ROW_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Id"), ID_COLUMN_INDEX, HEADER_ROW_INDEX);
			this.Controls.Add(NewTitleLabel("Title"), TITLE_COLUMN_INDEX, HEADER_ROW_INDEX);
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
			this.Controls.Add(rowNumberBox, ROW_COLUMN_INDEX, rowIndex);

			this.Controls.Add(NewDataLabel("Id", task.Id.ToString()), ID_COLUMN_INDEX, rowIndex);

			TextBox titleBox = NewTextBox("TitleTextBox", task.Title);
			titleBox.TextChanged += new EventHandler(titleTextBox_TextChanged);
			this.Controls.Add(titleBox, TITLE_COLUMN_INDEX, rowIndex);

			ComboBox statusComboBox = GenerateStatusComboBox(task.Status);
			this.Controls.Add(statusComboBox, STATUS_COLUMN_INDEX, rowIndex);

			ComboBox categoryComboBox = GenerateCategoryComboBox(task.Category);
			this.Controls.Add(categoryComboBox, CATEGORY_COLUMN_INDEX, rowIndex);

			this.Controls.Add(NewDataLabel("CreateDate", task.CreateDateString), CREATED_COLUMN_INDEX, rowIndex);
			this.Controls.Add(NewDataLabel("DoneDate", task.DoneDateString), DONE_COLUMN_INDEX, rowIndex);

			this.Controls.Add(NewButton("X", deleteTask_Click), DELETE_COLUMN_INDEX, rowIndex);

			this.RowCount++;

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
		}

		private void RemoveRow(int rowIndex)
		{
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
		}

		private void addTask_Click(object sender, EventArgs e)
		{
			//add task below current
			int row = this.GetRow(sender as Control) + 1;
			InsertTaskRowAt(row, project.InsertNewTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive));
		}

		private void deleteTask_Click(object sender, EventArgs e)
		{
			int row = this.GetRow(sender as Control);
			project.RemoveTask(IndexConverter.TableLayoutPanelToExcelWorksheet(row), active: showActive);
			RemoveRow(row);
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
		}

		private void titleTextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox textBox = (sender as TextBox);
			int row = this.GetRow(textBox);
			project.UpdateTitle(IndexConverter.TableLayoutPanelToExcelWorksheet(row), textBox.Text, active: showActive);
		}

		private void statusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);

			StatusChangeResult result = project.UpdateStatus(IndexConverter.TableLayoutPanelToExcelWorksheet(row), comboBox.Text, active: showActive);
			Label dateDoneLabel = (Label)this.GetControlFromPosition(7, row);
			dateDoneLabel.Text = result.DoneDateString;

			if(result.ActiveInactiveChanged)
			{
				RemoveRow(row);
			}
		}

		private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);
			project.UpdateCategory(IndexConverter.TableLayoutPanelToExcelWorksheet(row), comboBox.Text, active: showActive);
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
			textBox.TabIndex = 1;
			return textBox;
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
			comboBox.TabIndex = 2;
			return comboBox;
		}

		private Button NewButton(string text, EventHandler onClickHandler)
		{
			Button button = new Button();
			button.Font = regularFont;
			button.Location = new System.Drawing.Point(3, 3);
			button.Size = new System.Drawing.Size(19, 23);
			button.TabIndex = 7;
			button.Text = text;
			button.UseVisualStyleBackColor = true;
			button.Click += onClickHandler;
			return button;
		}
	}
}
